using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IJobQueryService
    {
        Task<SubmittedJob> GetJobById(long jobId);

        Task<SubmittedJob> GetJobById(long jobId, CancellationToken cancellationToken);

        Task<IEnumerable<SubmittedJob>> GetJobsAsync(
            long? ukprn = null,
            int? period = null,
            short? jobStatus = null,
            DateTime? startDateTimeUtc = null,
            DateTime? enDateTimeUtc = null,
            bool? isSubmitted = null,
            bool? isCollectionUploadType = null,
            string collectionType = null);

        Task<IEnumerable<SubmittedJob>> GetLatestJobsPerPeriodForHistoryAsync(long ukprn, CancellationToken cancellationToken);

        Task<IEnumerable<SubmittedJob>> GetLatestNCSJobsPerPeriodForHistoryAsync(long ukprn, CancellationToken cancellationToken);

        Task<IEnumerable<ProviderLatestSubmission>> GetLatestJobByUkprnAsync(long[] ukprns, CancellationToken cancellationToken);

        Task<IEnumerable<SubmittedJob>> GetJobsByUkprnAsync(long ukprn, CancellationToken cancellationToken);

        Task<IEnumerable<SubmittedJob>> GetJobsByUkprnPerCollectionAsync(long ukprn, string collectionName, CancellationToken cancellationToken);

        Task<IEnumerable<SubmittedJob>> GetJobsByUkprnForDateRangeAsync(long ukprn, DateTime startDateTimeUtc, DateTime endDateTimeUtc, CancellationToken cancellationToken);

        Task<IEnumerable<SubmittedJob>> GetJobsByUkprnForPeriodAsync(long ukprn, int period, CancellationToken cancellationToken);

        Task<IEnumerable<SubmittedJob>> GetLatestJobsPerPeriodByUkprnAsync(
            long ukprn,
            DateTime startDateTimeUtc,
            DateTime endDateTimeUtc,
            CancellationToken cancellationToken);

        Task<SubmittedJob> GetLatestJobByUkprnAsync(long ukprn, string collectionName, CancellationToken cancellationToken);

        Task<SubmittedJob> GetLatestJobByCollectionAsync(string collectionName, CancellationToken cancellationToken);

        Task<IEnumerable<SubmittedJob>> GetLatestSuccessfulJobByCollectionTypeByCollectioNameAsync(string collectionType, CancellationToken cancellationToken);

        Task<SubmittedJob> GetLatestJobByUkprnAndContractReferenceAsync(long ukprn, string contractReference, string collectionName, CancellationToken cancellationToken);

        Task<IEnumerable<SubmittedJob>> GetUnsubmittedIlrJobsAsync(long ukprn, CancellationToken cancellationToken);

        Task<SubmittedJob> GetLatestJobByUkprnAsync(long ukprn, string collectionName, int period, CancellationToken cancellationToken);

        Task<bool> IsAnyJobInProgressAsync(int collectionId, long? ukprn = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<FailedJob>> GetFailedJobsPerPeriodAsync(int collectionYear, int period, string collectionType = CollectionTypeConstants.Ilr, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<FileUploadJobMetaData>> GetJobsForAllPeriodsByCollectionAsync(string collectionName, short? statusCode, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<JobAndUkPrnDto>> GetAllSuccessfulJobsPerCollectionTypePerPeriodAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken);

        Task<IEnumerable<JobMetaDataDto>> GetAllJobsPerCollectionPerPeriodAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken);

        Task<IEnumerable<JobMetaDataDto>> GetAllJobsPerCollectionAsync(string collectionName, CancellationToken cancellationToken);

        Task<IEnumerable<JobMetaDataDto>> GetPublishJobsForCollectionAsync(string collectionName, CancellationToken cancellationToken);

        Task<IEnumerable<string>> GetCollectionTypesWithSubmissionsForProviderAsync(long ukprn, CancellationToken cancellationToken);
    }
}