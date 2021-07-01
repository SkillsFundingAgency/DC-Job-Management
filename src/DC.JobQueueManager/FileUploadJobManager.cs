using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Converters;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.JobsMetaDataManager;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using Job = ESFA.DC.JobQueueManager.Data.Entities.Job;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.JobQueueManager
{
    public sealed class FileUploadJobManager : AbstractJobManager, IFileUploadJobManager
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJobEmailTemplateManager _jobEmailTemplateManager;
        private readonly ILogger _logger;
        private readonly ICollectionService _collectionService;
        private readonly IJobConverter _jobConverter;
        private readonly IJobMetaDataManager[] _jobMetaDataManagers;
        private readonly IJobManager _jobManager;

        public FileUploadJobManager(
            Func<IJobQueueDataContext> contextFactory,
            IDateTimeProvider dateTimeProvider,
            IJobEmailTemplateManager jobEmailTemplateManager,
            ILogger logger,
            ICollectionService collectionService,
            IJobConverter jobConverter,
            IJobMetaDataManager[] jobMetaDataManagers,
            IJobManager jobManager)
        : base(contextFactory)
        {
            _contextFactory = contextFactory;
            _dateTimeProvider = dateTimeProvider;
            _jobEmailTemplateManager = jobEmailTemplateManager;
            _logger = logger;
            _collectionService = collectionService;
            _jobConverter = jobConverter;
            _jobMetaDataManagers = jobMetaDataManagers;
            _jobManager = jobManager;
        }

        public async Task<long> AddJob(FileUploadJob job)
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

            using (var context = _contextFactory())
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

                foreach (var jobMetaDataManager in _jobMetaDataManagers)
                {
                    jobMetaDataManager.AddMetaData(entity, job, context);
                }

                await context.SaveChangesAsync();

                // send email on create for esf and eas
                if (collection.EmailOnJobCreation)
                {
                    await _jobEmailTemplateManager.SendEmailNotification(entity.JobId);
                }

                return entity.JobId;
            }
        }

        public async Task<bool> SubmitIlrJob(long jobId)
        {
            using (var context = _contextFactory())
            {
                var entity = await context.Job.SingleOrDefaultAsync(x => x.JobId == jobId);
                if (entity == null)
                {
                    throw new ArgumentException($"Job id {jobId} does not exist");
                }

                try
                {
                    var data = await context.FromSqlAsync<int>(
                        CommandType.StoredProcedure,
                        "dbo.SubmitJobCreateIlrJobMetaData",
                        new { jobId, statusDateTime = _dateTimeProvider.GetNowUtc() });

                    if (data.Any() && data.First() == 0)
                    {
                        throw new DbUpdateConcurrencyException("Save failed.", null);
                    }

                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new Exception(
                        "Save failed. Job details have been changed. Reload the job object and try save again");
                }
            }
        }

        public async Task<IEnumerable<FileUploadJob>> GetAllJobs()
        {
            using (var context = _contextFactory())
            {
                return await context.FileUploadJobMetaData
                    .Include(x => x.Job)
                    .ThenInclude(x => x.Collection)
                    .Select(md => new FileUploadJob
                    {
                        FileName = md.FileName,
                        FileSize = md.FileSize.GetValueOrDefault(0),
                        StorageReference = md.StorageReference,
                        JobId = md.JobId,
                        CollectionName = md.Job.Collection.Name,
                        PeriodNumber = md.PeriodNumber,
                        Ukprn = md.Job.Ukprn.GetValueOrDefault(),
                        DateTimeSubmittedUtc = md.Job.DateTimeCreatedUtc,
                        Priority = md.Job.Priority,
                        Status = (JobStatusType)md.Job.Status,
                        DateTimeUpdatedUtc = md.Job.DateTimeUpdatedUtc,
                        RowVersion = md.Job.RowVersion == null ? null : System.Convert.ToBase64String(md.Job.RowVersion),
                        CreatedBy = md.Job.CreatedBy,
                        NotifyEmail = md.Job.NotifyEmail,
                        CrossLoadingStatus = md.Job.CrossLoadingStatus.HasValue ? (JobStatusType)md.Job.CrossLoadingStatus.Value : (JobStatusType?)null,
                        DateTimeCreatedUtc = md.Job.DateTimeCreatedUtc
                    })
                    .ToListAsync();
            }
        }

        public async Task<bool> UpdateJob(FileUploadJob job)
            => await _jobManager.UpdateJob(job);
    }
}