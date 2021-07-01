using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Converters;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using Job = ESFA.DC.JobQueueManager.Data.Entities.Job;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.JobQueueManager
{
    public sealed class JobManager : AbstractJobManager, IJobManager
    {
        private readonly HashSet<JobStatusType> _validUpdateJobStatusTypes = new HashSet<JobStatusType> { JobStatusType.Failed, JobStatusType.FailedRetry, JobStatusType.Waiting };

        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IEmailNotifier _emailNotifier;
        private readonly IJobEmailTemplateManager _jobEmailTemplateManager;
        private readonly ILogger _logger;
        private readonly ICollectionService _collectionService;
        private readonly IJobConverter _jobConverter;
        private readonly IJobQueryService _jobQueryService;
        private readonly SemaphoreSlim _updateLock;

        public JobManager(
            Func<IJobQueueDataContext> contextFactory,
            IDateTimeProvider dateTimeProvider,
            IEmailNotifier emailNotifier,
            IJobEmailTemplateManager jobEmailTemplateManager,
            ILogger logger,
            ICollectionService collectionService,
            IJobConverter jobConverter,
            IJobQueryService jobQueryService)
            : base(contextFactory)
        {
            _contextFactory = contextFactory;
            _dateTimeProvider = dateTimeProvider;
            _emailNotifier = emailNotifier;
            _jobEmailTemplateManager = jobEmailTemplateManager;
            _logger = logger;
            _collectionService = collectionService;
            _jobConverter = jobConverter;
            _jobQueryService = jobQueryService;
            _updateLock = new SemaphoreSlim(1, 1);
        }

        public async Task<long> AddJob(Jobs.Model.Job job)
        {
            if (job == null)
            {
                throw new ArgumentNullException();
            }

            var collection = await _collectionService.GetCollectionFromNameAsync(CancellationToken.None, job.CollectionName);

            if (collection == null)
            {
                throw new ArgumentException($"collection name is not valid : collection name : {job.CollectionName}");
            }

            using (IJobQueueDataContext context = _contextFactory())
            {
                var entity = new Job
                {
                    DateTimeCreatedUtc = _dateTimeProvider.GetNowUtc(),
                    CollectionId = collection.CollectionId,
                    Priority = job.Priority,
                    Status = (short)job.Status,
                    CreatedBy = job.CreatedBy,
                    NotifyEmail = job.NotifyEmail,
                    CrossLoadingStatus = (await IsCrossLoadingEnabled(collection.CollectionId)) ? (short)JobStatusType.Ready : (short?)null,
                    Ukprn = job.Ukprn,
                };
                context.Job.Add(entity);
                context.SaveChanges();
                return entity.JobId;
            }
        }

        public async Task<IEnumerable<Jobs.Model.Job>> GetAllJobs()
        {
            var jobs = new List<Jobs.Model.Job>();
            using (IJobQueueDataContext context = _contextFactory())
            {
                var jobEntities = await context.Job.Include(x => x.Collection).ToListAsync();
                foreach (var jobEntity in jobEntities)
                {
                    jobs.Add(await _jobConverter.Convert(jobEntity));
                }
            }

            return jobs;
        }

        public async Task<IEnumerable<Jobs.Model.Job>> GetJobsByPriorityAsync(int resultCount)
        {
            List<Jobs.Model.Job> jobs = new List<Jobs.Model.Job>();
            using (IJobQueueDataContext context = _contextFactory())
            {
                Job[] jobEntities = await context.Job.FromSql("dbo.GetJobByPriority @ResultCount={0}", resultCount).ToArrayAsync();

                foreach (Job jobEntity in jobEntities)
                {
                    jobs.Add(await _jobConverter.Convert(jobEntity));
                }
            }

            return jobs;
        }

        public async Task RemoveJobFromQueue(long jobId)
        {
            if (jobId == 0)
            {
                throw new ArgumentException("Job id can not be 0");
            }

            using (IJobQueueDataContext context = _contextFactory())
            {
                var jobEntity = await context.Job.SingleOrDefaultAsync(x => x.JobId == jobId);
                if (jobEntity == null)
                {
                    throw new ArgumentException($"Job id {jobId} does not exist");
                }

                if (jobEntity.Status != 1) // if already moved, then dont delete
                {
                    throw new ArgumentOutOfRangeException("Job is already moved from ready status, unable to delete");
                }

                context.Job.Remove(jobEntity);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdateJob(Jobs.Model.Job job)
        {
            if (job == null)
            {
                throw new ArgumentNullException();
            }

            await _updateLock.WaitAsync();

            try
            {
                using (IJobQueueDataContext context = _contextFactory())
                {
                    Job entity = await context.Job.Include(x => x.Collection).SingleOrDefaultAsync(x => x.JobId == job.JobId);
                    if (entity == null)
                    {
                        throw new ArgumentException($"Job id {job.JobId} does not exist");
                    }

                    bool statusChanged = entity.Status != (short)job.Status;

                    await _jobConverter.Convert(job, entity);
                    entity.DateTimeUpdatedUtc = _dateTimeProvider.GetNowUtc();
                    entity.RowVersion = job.RowVersion == null ? null : Convert.FromBase64String(job.RowVersion);

                    context.Entry(entity).State = EntityState.Modified;

                    if (job.Status == JobStatusType.Ready)
                    {
                        context.IlrJobMetaData.Add(new IlrJobMetaData()
                        {
                            DateTimeSubmittedUtc = _dateTimeProvider.GetNowUtc(),
                            JobId = job.JobId,
                        });
                    }

                    try
                    {
                        await context.SaveChangesAsync();

                        if (statusChanged)
                        {
                            await SendEmailNotification(job.JobId);
                        }
                    }
                    catch (DbUpdateConcurrencyException exception)
                    {
                        throw new Exception(
                            "Save failed. Job details have been changed. Reload the job object and try save again");
                    }
                }
            }
            finally
            {
                _updateLock.Release();
            }

            return true;
        }

        public async Task<bool> UpdateJobStatus(long jobId, JobStatusType status)
        {
            if (jobId == 0)
            {
                throw new ArgumentException("Job id can not be 0");
            }

            await _updateLock.WaitAsync();

            try
            {
                using (IJobQueueDataContext context = _contextFactory())
                {
                    var entity = await context.Job
                        .Include(x => x.Collection)
                        .ThenInclude(x => x.CollectionType)
                        .SingleOrDefaultAsync(x => x.JobId == jobId);
                    if (entity == null)
                    {
                        throw new ArgumentException($"Job id {jobId} does not exist");
                    }

                    var statusChanged = entity.Status != (short)status;

                    // if its not allowed status update then return true and do not execute the update
                    if (statusChanged && !IsStatusUpdateAllowed(jobId, (JobStatusType)entity.Status, status))
                    {
                        return true;
                    }

                    var datetimeUpdated = _dateTimeProvider.GetNowUtc();

                    entity.Status = (short)status;
                    entity.DateTimeUpdatedUtc = datetimeUpdated;
                    context.Entry(entity).State = EntityState.Modified;

                    if (status == JobStatusType.Ready && entity.Collection.MultiStageProcessing)
                    {
                        var data = await context.FromSqlAsync<int>(
                            CommandType.StoredProcedure,
                            "dbo.SubmitJobCreateIlrJobMetaData",
                            new { jobId, statusDateTime = _dateTimeProvider.GetNowUtc() });

                        if (data.Any() && data.First() == 0)
                        {
                            _logger.LogError($"Job:{jobId} failed to update IlrJobMetaData.");
                        }
                    }

                    await context.SaveChangesAsync();

                    if (statusChanged)
                    {
                        await SendEmailNotification(jobId);
                    }
                }
            }
            finally
            {
                _updateLock.Release();
            }

            return true;
        }

        public async Task<bool> UpdateCrossLoadingStatus(long jobId, JobStatusType status, CancellationToken cancellationToken)
        {
            if (jobId == 0)
            {
                throw new ArgumentException("Job id can not be 0");
            }

            await _updateLock.WaitAsync(cancellationToken);

            try
            {
                using (IJobQueueDataContext context = _contextFactory())
                {
                    var entity = await context.Job.SingleOrDefaultAsync(x => x.JobId == jobId, cancellationToken);
                    if (entity == null)
                    {
                        throw new ArgumentException($"Job id {jobId} does not exist");
                    }

                    entity.CrossLoadingStatus = (short)status;
                    entity.DateTimeUpdatedUtc = _dateTimeProvider.GetNowUtc();
                    context.Entry(entity).State = EntityState.Modified;

                    await context.SaveChangesAsync(cancellationToken);
                }
            }
            finally
            {
                _updateLock.Release();
            }

            return true;
        }

        public async Task<long> CloneJob(long jobId, CancellationToken cancellationToken)
        {
            long newJobId;

            if (jobId <= 0)
            {
                throw new ArgumentNullException();
            }

            using (IJobQueueDataContext context = _contextFactory())
            {
                var existingJob = await context.Job
                    .Include(x => x.FileUploadJobMetaData)
                    .Include(x => x.EasJobMetaData)
                    .Include(x => x.EsfJobMetaData)
                    .Include(x => x.IlrJobMetaData)
                    .Include(x => x.ReportsPublicationJobMetaData)
                    .Include(x => x.NcsJobMetaData)
                    .Include(x => x.PathItemJob)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.JobId == jobId, cancellationToken);
                if (existingJob == null)
                {
                    throw new ArgumentException("Job id is not valid");
                }

                var newJob = (Job)context.Entry(existingJob).CurrentValues.ToObject();

                var nowDateTime = _dateTimeProvider.GetNowUtc();

                newJob.JobId = 0;
                newJob.DateTimeUpdatedUtc = nowDateTime;
                newJob.DateTimeCreatedUtc = nowDateTime;
                newJob.Status = (short)JobStatusType.Ready;
                newJob.NotifyEmail = null;
                newJob.CreatedBy = "ESFA";

                ////set related entities
                if (existingJob.FileUploadJobMetaData.Any())
                {
                    var childEntity = existingJob.FileUploadJobMetaData.First();
                    childEntity.Id = 0;
                    newJob.FileUploadJobMetaData.Add(childEntity);
                }

                if (existingJob.EasJobMetaData.Any())
                {
                    var childEntity = existingJob.EasJobMetaData.First();
                    childEntity.Id = 0;
                    newJob.EasJobMetaData.Add(childEntity);
                }

                if (existingJob.EsfJobMetaData.Any())
                {
                    var childEntity = existingJob.EsfJobMetaData.First();
                    childEntity.Id = 0;
                    newJob.EsfJobMetaData.Add(childEntity);
                }

                if (existingJob.IlrJobMetaData.Any())
                {
                    newJob.IlrJobMetaData.Add(new IlrJobMetaData()
                    {
                        DateTimeSubmittedUtc = _dateTimeProvider.GetNowUtc()
                    });
                }

                if (existingJob.NcsJobMetaData.Any())
                {
                    var childEntity = existingJob.NcsJobMetaData.First();
                    childEntity.Id = 0;
                    newJob.NcsJobMetaData.Add(childEntity);
                }

                if (existingJob.ReportsPublicationJobMetaData.Any())
                {
                    var childEntity = existingJob.ReportsPublicationJobMetaData.First();
                    childEntity.Id = 0;
                    newJob.ReportsPublicationJobMetaData.Add(childEntity);
                }

                context.Job.Add(newJob);
                await context.SaveChangesAsync(cancellationToken);

                newJobId = newJob.JobId;

                // If an entry exists on the PathItemJob table for the old job then add a new row for the new jobId
                if (existingJob.PathItemJob.Any())
                {
                    var childEntity = existingJob.PathItemJob.First();
                    childEntity.JobId = newJobId;
                    newJob.PathItemJob.Add(childEntity);
                    await context.SaveChangesAsync(cancellationToken);
                }
            }

            return newJobId;
        }

        public async Task SendEmailNotification(SubmittedJob job)
        {
            try
            {
                var template = await _jobEmailTemplateManager.GetTemplate(job.JobId, job.DateTimeSubmittedUtc);

                if (!string.IsNullOrEmpty(template))
                {
                    var personalisation = new Dictionary<string, dynamic>();

                    var submittedAt = _dateTimeProvider.ConvertUtcToUk(job.DateTimeSubmittedUtc);
                    personalisation.Add("JobId", job.JobId);
                    personalisation.Add("Name", job.CreatedBy);
                    personalisation.Add("DateTimeSubmitted", string.Concat(submittedAt.ToString("hh:mm tt"), " on ", submittedAt.ToString("dddd dd MMMM yyyy")));

                    await _emailNotifier.SendEmail(job.NotifyEmail, template, personalisation);

                    _logger.LogInfo($"Sent email for jobId : {job.JobId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Sending email failed for job {job.JobId}", ex, jobIdOverride: job.JobId);
            }
        }

        public async Task SendEmailNotification(long jobId)
        {
            var job = await _jobQueryService.GetJobById(jobId);
            await SendEmailNotification(job);
        }

        private bool IsStatusUpdateAllowed(long jobId, JobStatusType existingStatus, JobStatusType newStatus)
        {
            var isInvalidReadyStatusUpdate = newStatus == JobStatusType.Ready && !_validUpdateJobStatusTypes.Contains(existingStatus);

            if (existingStatus == JobStatusType.Completed || isInvalidReadyStatusUpdate)
            {
                _logger.LogInfo($"Invalid status update, update not executed - Status changing from {existingStatus} to {newStatus} for job id : {jobId}", null, jobIdOverride: jobId);
                return false;
            }

            return true;
        }
    }
}