using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Data.ReadOnlyEntities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.JobQueueManager
{
    public class JobQueryService : IJobQueryService
    {
        private const int ReadyStatusCode = 1;
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly Func<IOrganisationsContext> _orgContextFactory;
        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public JobQueryService(
            Func<IJobQueueDataContext> contextFactory,
            Func<IOrganisationsContext> orgContextFactory,
            ILogger logger,
            IDateTimeProvider dateTimeProvider)
        {
            _contextFactory = contextFactory;
            _orgContextFactory = orgContextFactory;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<SubmittedJob> GetJobById(long jobId)
        {
            if (jobId == 0)
            {
                throw new ArgumentException("Job id can not be 0");
            }

            using (IJobQueueDataContext context = _contextFactory())
            {
                var entity = await context.ReadOnlyJob.Where(x => x.JobId == jobId).ToListAsync();
                if (entity == null)
                {
                    throw new ArgumentException($"Job id {jobId} does not exist");
                }

                return ConvertJobs(entity).FirstOrDefault();
            }
        }

        public async Task<SubmittedJob> GetJobById(long jobId, CancellationToken cancellationToken)
        {
            if (jobId == 0)
            {
                throw new ArgumentException("Job id can not be 0");
            }

            using (IJobQueueDataContext context = _contextFactory())
            {
                var entity = await context.ReadOnlyJob.Where(x => x.JobId == jobId).ToListAsync(cancellationToken);
                if (entity == null)
                {
                    throw new ArgumentException($"Job id {jobId} does not exist");
                }

                return ConvertJobs(entity).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<SubmittedJob>> GetJobsAsync(
            long? ukprn = null,
            int? period = null,
            short? jobStatus = null,
            DateTime? startDateTimeUtc = null,
            DateTime? endDateTimeUtc = null,
            bool? isSubmitted = null,
            bool? isCollectionUploadType = null,
            string collectionType = null)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    IQueryable<ReadOnlyJob> queryable = context.ReadOnlyJob.AsQueryable();

                    if (string.IsNullOrEmpty(collectionType) && isSubmitted.HasValue)
                    {
                        queryable = queryable.Where(x => x.IsSubmitted == isSubmitted.Value);
                    }

                    if (ukprn.HasValue)
                    {
                        queryable = queryable.Where(x => x.Ukprn == ukprn.Value);
                    }

                    if (period.HasValue)
                    {
                        queryable = queryable.Where(x => x.PeriodNumber == period.Value);
                    }

                    if (jobStatus.HasValue)
                    {
                        queryable = queryable.Where(x => x.Status == jobStatus.Value);
                    }

                    if (startDateTimeUtc.HasValue)
                    {
                        queryable = queryable.Where(x => x.DateTimeSubmittedUtc >= startDateTimeUtc.Value);
                    }

                    if (endDateTimeUtc.HasValue)
                    {
                        queryable = queryable.Where(x => x.DateTimeSubmittedUtc <= endDateTimeUtc.Value);
                    }

                    if (string.IsNullOrEmpty(collectionType) && isCollectionUploadType.HasValue)
                    {
                        queryable = queryable.Where(x => x.IsCollectionUploadType == isCollectionUploadType.Value);
                    }

                    if (!string.IsNullOrEmpty(collectionType))
                    {
                        queryable = queryable.Where(x => x.CollectionType.Equals(collectionType, StringComparison.OrdinalIgnoreCase));
                    }

                    var data = await queryable
                        .OrderByDescending(x => x.DateTimeSubmittedUtc)
                        .ToListAsync();

                    return ConvertJobs(data.ToList());
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting jobs list for ukprn : {ukprn}", e);
                throw;
            }
        }

        public async Task<IEnumerable<FileUploadJobMetaData>> GetJobsForAllPeriodsByCollectionAsync(string collectionName, short? statusCode, CancellationToken cancellationToken)
        {
            if (!statusCode.HasValue)
            {
                statusCode = (short)JobStatusType.Ready;
            }

            using (var context = _contextFactory())
            {
                return await context.FileUploadJobMetaData
                    .Include(i => i.Job)
                    .ThenInclude(t => t.Collection)
                    .Where(w => w.Job.Collection.Name == collectionName && w.Job.Status == statusCode.Value)
                    .OrderBy(x => x.JobId)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<SubmittedJob>> GetLatestJobsPerPeriodForHistoryAsync(long ukprn, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var data = await context.FromSqlAsync<ReadOnlyJob>(
                        CommandType.StoredProcedure,
                        "GetLatestJobPerPeriod",
                        new { ukprn, currentDateTimeUtc = _dateTimeProvider.GetNowUtc() });

                    return ConvertJobs(data);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting GetLatestJobsPerPeriodForHistoryAsync for ukprn : {ukprn}", e);
                throw;
            }
        }

        public async Task<IEnumerable<SubmittedJob>> GetLatestNCSJobsPerPeriodForHistoryAsync(long ukprn, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var data = await context.FromSqlAsync<ReadOnlyJob>(
                        CommandType.StoredProcedure,
                        "GetLatestNCSJobPerPeriod",
                        new { ukprn, currentDateTimeUtc = _dateTimeProvider.GetNowUtc() });

                    return ConvertJobs(data);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting GetLatestNCSJobsPerPeriodForHistoryAsync for ukprn : {ukprn}", e);
                throw;
            }
        }

        public async Task<IEnumerable<ProviderLatestSubmission>> GetLatestJobByUkprnAsync(long[] ukprns, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var result = await context.FromSqlAsync<ProviderLatestSubmission>(
                    CommandType.StoredProcedure,
                    "GetLatestJobs",
                    new { ukprns = JsonConvert.SerializeObject(ukprns) });

                return result;
            }
        }

        public async Task<IEnumerable<SubmittedJob>> GetJobsByUkprnAsync(long ukprn, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var entities = await context.ReadOnlyJob.Where(x => x.Ukprn == ukprn)
                    .ToListAsync();
                return ConvertJobs(entities);
            }
        }

        public async Task<IEnumerable<SubmittedJob>> GetJobsByUkprnPerCollectionAsync(long ukprn, string collectionName, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var entities = await context.ReadOnlyJob.Where(x => x.Ukprn == ukprn && x.CollectionName.Equals(collectionName, StringComparison.CurrentCultureIgnoreCase))
                    .ToListAsync();
                return ConvertJobs(entities);
            }
        }

        public async Task<IEnumerable<SubmittedJob>> GetJobsByUkprnForDateRangeAsync(long ukprn, DateTime startDateTimeUtc, DateTime endDateTimeUtc, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var entities = await context.ReadOnlyJob
                    .Where(x => x.Ukprn == ukprn &&
                                x.IsSubmitted &&
                                x.IsCollectionUploadType &&
                                x.DateTimeSubmittedUtc >= startDateTimeUtc &&
                                x.DateTimeSubmittedUtc <= endDateTimeUtc)
                    .ToListAsync();
                return ConvertJobs(entities);
            }
        }

        public async Task<IEnumerable<SubmittedJob>> GetJobsByUkprnForPeriodAsync(long ukprn, int period, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var entities = await context.ReadOnlyJob
                    .Where(x => x.Ukprn == ukprn && x.IsCollectionUploadType && x.PeriodNumber == period)
                    .ToListAsync();
                return ConvertJobs(entities);
            }
        }

        public async Task<IEnumerable<SubmittedJob>> GetLatestJobsPerPeriodByUkprnAsync(
            long ukprn,
            DateTime startDateTimeUtc,
            DateTime endDateTimeUtc,
            CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var entities = await context.ReadOnlyJob
                    .Where(x => x.Ukprn == ukprn &&
                                x.IsCollectionUploadType &&
                                x.Status == (short)JobStatusType.Completed &&
                                x.DateTimeSubmittedUtc >= startDateTimeUtc &&
                                x.DateTimeSubmittedUtc <= endDateTimeUtc)
                    .GroupBy(x => new { x.CollectionId, x.PeriodNumber })
                    .Select(g => g.OrderByDescending(x => x.DateTimeSubmittedUtc).FirstOrDefault())
                    .ToListAsync();
                return ConvertJobs(entities);
            }
        }

        public async Task<SubmittedJob> GetLatestJobByUkprnAsync(long ukprn, string collectionName, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var entity = await context.ReadOnlyJob
                    .Where(x => x.Ukprn == ukprn && x.Status == (short)JobStatusType.Completed &&
                                x.CollectionName.Equals(collectionName, StringComparison.CurrentCultureIgnoreCase))
                    .OrderByDescending(x => x.DateTimeSubmittedUtc).ToListAsync();

                return ConvertJobs(entity).FirstOrDefault();
            }
        }

        public async Task<SubmittedJob> GetLatestJobByCollectionAsync(string collectionName, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var entity = await context.ReadOnlyJob
                    .Where(x => x.CollectionName == collectionName)
                    .OrderByDescending(x => x.DateTimeSubmittedUtc)
                    .ToListAsync(cancellationToken);

                return !entity.Any() ? null : ConvertJobs(entity).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<SubmittedJob>> GetLatestSuccessfulJobByCollectionTypeByCollectioNameAsync(string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var entity = await context.ReadOnlyJob
                    .Where(x => x.Status == (short)JobStatusType.Completed &&
                                x.CollectionType == collectionType)
                    .GroupBy(x => x.CollectionName)
                    .Select(g => g.OrderByDescending(x => x.DateTimeSubmittedUtc).FirstOrDefault())
                    .OrderBy(j => j.CollectionName)
                    .ToListAsync(cancellationToken);

                return !entity.Any() ? null : ConvertJobs(entity);
            }
        }

        public async Task<SubmittedJob> GetLatestJobByUkprnAsync(long ukprn, string collectionName, int period, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var entity = await context.ReadOnlyJob
                    .Where(x => x.Ukprn == ukprn &&
                                x.Status == (short)JobStatusType.Completed &&
                                x.PeriodNumber == period &&
                                x.CollectionName.Equals(collectionName, StringComparison.CurrentCultureIgnoreCase))
                    .OrderByDescending(x => x.DateTimeSubmittedUtc).ToListAsync();

                return ConvertJobs(entity).FirstOrDefault();
            }
        }

        public async Task<SubmittedJob> GetLatestJobByUkprnAndContractReferenceAsync(long ukprn, string contractReference, string collectionName, CancellationToken cancellationToken)
        {
            var fileNameSearchQuery = $"{ukprn}/SUPPDATA-{ukprn}-{contractReference}-";
            using (var context = _contextFactory())
            {
                var entity = await context.ReadOnlyJob
                    .Where(
                        x => x.Ukprn == ukprn &&
                             x.CollectionName.Equals(collectionName, StringComparison.CurrentCultureIgnoreCase) &&
                             x.FileName.StartsWith(fileNameSearchQuery))
                    .OrderByDescending(x => x.DateTimeSubmittedUtc).ToListAsync();

                return ConvertJobs(entity).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<SubmittedJob>> GetUnsubmittedIlrJobsAsync(long ukprn, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var result = await context.FromSqlAsync<ReadOnlyJob>(
                    CommandType.StoredProcedure,
                    "GetUnSubmittedIlrJobs",
                    new { ukprn });

                return ConvertJobs(result);
            }
        }

        public async Task<IEnumerable<SubmittedJob>> GetAllJobsAsync(CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var entities = await context.ReadOnlyJob.ToListAsync();
                return ConvertJobs(entities);
            }
        }

        public async Task<bool> IsAnyJobInProgressAsync(int collectionId, long? ukprn = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            short[] inProgressStatus = { (short)JobStatusType.Processing, (short)JobStatusType.Ready, (short)JobStatusType.MovedForProcessing };

            using (var context = _contextFactory())
            {
                var result = await context.Job.AnyAsync(x => x.CollectionId == collectionId &&
                                                             inProgressStatus.Contains(x.Status) &&
                                                             (ukprn == null || x.Ukprn == ukprn));
                return result;
            }
        }

        public IEnumerable<SubmittedJob> ConvertJobs(IEnumerable<ReadOnlyJob> entities)
        {
            var items = new List<SubmittedJob>();
            foreach (var entity in entities)
            {
                items.Add(new SubmittedJob()
                {
                    CollectionName = entity.CollectionName,
                    CalendarMonth = entity.CalendarMonth,
                    CalendarYear = entity.CalendarYear,
                    CreatedBy = entity.CreatedBy,
                    DateTimeSubmittedUtc = entity.DateTimeSubmittedUtc,
                    DateTimeCreatedUtc = entity.DateTimeCreatedUtc,
                    FileName = entity.FileName,
                    JobId = entity.JobId,
                    PeriodNumber = entity.PeriodNumber,
                    Status = (JobStatusType)entity.Status,
                    Ukprn = entity.Ukprn,
                    CollectionId = entity.CollectionId,
                    CollectionType = entity.CollectionType,
                    CollectionYear = entity.CollectionYear,
                    IsSubmitted = entity.IsSubmitted,
                    NotifyEmail = entity.NotifyEmail,
                    FileSize = entity.FileSize,
                    StorageReference = entity.StorageReference,
                    ExternalJobId = entity.ExternalJobId,
                    ReportFileName = entity.ReportFileName,
                    TouchpointId = entity.TouchpointId,
                    ExternalTimestamp = entity.ExternalTimestamp,
                    ContractReferenceNumber = entity.ContractReferenceNumber,
                    SourceFolderKey = entity.SourceFolderKey,
                    SourceContainerName = entity.SourceContainerName,
                    DssContainer = entity.DssContainer,
                    ReportEndDate = entity.ReportEndDate,
                    Rule = entity.Rule,
                    SelectedCollectionYear = entity.SelectedCollectionYear
                });
            }

            return items;
        }

        public async Task<IEnumerable<FailedJob>> GetFailedJobsPerPeriodAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            var failedJobs = new List<FailedJob>();

            using (var context = _contextFactory())
            using (var orgContext = _orgContextFactory())
            {
                var result = (await context.FromSqlAsync<FailedJob>(
                        CommandType.StoredProcedure,
                        "GetLatestFailedJobsPerCollectionPerPeriod",
                        new { collectionYear, periodNumber, collectionType })).ToList();

                foreach (var failedJob in result)
                {
                    failedJob.OrganisationName =
                        orgContext.OrgDetails.FirstOrDefault(o => o.Ukprn == failedJob.Ukprn)?.Name;
                    failedJob.FileName = !string.IsNullOrEmpty(failedJob.FileName) && failedJob.FileName.Contains("/")
                        ? failedJob.FileName.Substring(failedJob.FileName.LastIndexOf('/') + 1)
                        : failedJob.FileName;
                    failedJob.DateTimeSubmitted = _dateTimeProvider.ConvertUtcToUk(failedJob.DateTimeSubmitted ?? DateTime.MinValue);
                    failedJobs.Add(failedJob);
                }
            }

            return failedJobs;
        }

        public async Task<IEnumerable<JobAndUkPrnDto>> GetAllSuccessfulJobsPerCollectionTypePerPeriodAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            List<JobAndUkPrnDto> jobs;
            using (var context = _contextFactory())
            {
                jobs = (await context.FromSqlAsync<JobAndUkPrnDto>(
                    CommandType.StoredProcedure,
                    "GetAllSuccessfulJobsPerCollectionTypePerPeriod",
                    new { collectionYear, periodNumber, collectionType })).ToList();
            }

            return jobs;
        }

        public async Task<IEnumerable<JobMetaDataDto>> GetAllJobsPerCollectionPerPeriodAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.FileUploadJobMetaData
                    .Where(m => m.Job.Collection.CollectionType.Type == collectionType
                                && (m.Job.Collection.CollectionYear == collectionYear || collectionYear == 0)
                                && (m.PeriodNumber == periodNumber || periodNumber == 0))
                    .Select(m => new JobMetaDataDto
                    {
                        JobId = m.JobId,
                        JobStatus = m.Job.Status,
                        PeriodNumber = m.PeriodNumber,
                        FileName = m.FileName,
                        SubmittedBy = m.Job.CreatedBy,
                        SubmissionDate = m.Job.DateTimeCreatedUtc
                    }).ToListAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<JobMetaDataDto>> GetJobsWithNoFilePerCollectionAsync(string collectionName, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.Job
                    .Where(j => j.Collection.Name == collectionName)
                    .Select(j => new JobMetaDataDto
                    {
                        JobId = j.JobId,
                        JobStatus = j.Status,
                        PeriodNumber = -1,
                        FileName = string.Empty,
                        SubmittedBy = j.CreatedBy,
                        SubmissionDate = j.DateTimeCreatedUtc
                    }).ToListAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<JobMetaDataDto>> GetAllJobsPerCollectionAsync(string collectionName, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.FileUploadJobMetaData
                    .Where(m => m.Job.Collection.Name == collectionName)
                    .Select(m => new JobMetaDataDto
                    {
                        JobId = m.JobId,
                        JobStatus = m.Job.Status,
                        PeriodNumber = m.PeriodNumber,
                        FileName = m.FileName,
                        SubmittedBy = m.Job.CreatedBy,
                        SubmissionDate = m.Job.DateTimeCreatedUtc
                    }).ToListAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<JobMetaDataDto>> GetPublishJobsForCollectionAsync(string collectionName, CancellationToken cancellationToken)
        {
            using (
                var context = _contextFactory())
            {
                return await context.Job
                    .Where(m => m.Collection.Name == collectionName)
                    .Select(m => new JobMetaDataDto
                    {
                        JobId = m.JobId,
                        JobStatus = m.Status,
                        SubmittedBy = m.CreatedBy,
                        SubmissionDate = m.DateTimeCreatedUtc,
                        VersionNumber = m.FisJobMetaData.FirstOrDefault().VersionNumber,
                        EsfPublishedDate = m.EsfJobMetaData.FirstOrDefault().PublishedDate,
                        FisPublishedDate = m.FisJobMetaData.FirstOrDefault().PublishedDate,
                        FisRemoveFlag = m.FisJobMetaData.FirstOrDefault().IsRemoved
                    }).ToListAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<string>> GetCollectionTypesWithSubmissionsForProviderAsync(long ukprn, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.Job
                    .Include(x => x.Collection)
                    .ThenInclude(x => x.CollectionType)
                    .Where(m => m.Ukprn == ukprn)
                    .Select(x => x.Collection.CollectionType.Type)
                    .Distinct()
                    .ToListAsync(cancellationToken);
            }
        }
    }
}