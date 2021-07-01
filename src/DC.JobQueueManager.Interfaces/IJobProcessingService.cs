using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Api.Common.Utilities.Pagination;
using ESFA.DC.Jobs.Model.Processing.DasMismatch;
using ESFA.DC.Jobs.Model.Processing.Detail;
using ESFA.DC.Jobs.Model.Processing.JobsConcern;
using ESFA.DC.Jobs.Model.Processing.JobsFailedCurrentPeriod;
using ESFA.DC.Jobs.Model.Processing.JobsFailedToday;
using ESFA.DC.Jobs.Model.Processing.JobsProcessing;
using ESFA.DC.Jobs.Model.Processing.JobsQueued;
using ESFA.DC.Jobs.Model.Processing.JobsSlowFile;
using ESFA.DC.Jobs.Model.Processing.JobsSubmitted;
using ESFA.DC.Jobs.Model.Processing.ProvidersReturned;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IJobProcessingService
    {
        Task<JobsProcessingModel> GetJobsThatAreProcessing(DateTime dateTime);

        Task<JobsQueuedModel> GetJobsThatAreQueued(DateTime dateTime);

        Task<JobsSubmittedModel> GetJobsThatAreSubmitted(DateTime dateTime);

        Task<JobsFailedTodayModel> GetJobsThatAreFailedToday(DateTime dateTime);

        Task<JobsSlowFileModel> GetJobsThatAreSlowFile(DateTime dateTime);

        Task<PaginatedResult<JobDetails>> GetPaginatedJobsInDetail(short jobStatus, DateTime startDateTimeUtc, DateTime endDateTimeUtc, int pageSize = int.MaxValue, int pageNumber = 1);

        Task<JobConcernModel> GetJobsThatAreConcern(DateTime dateTime);

        Task<PaginatedResult<JobDetails>> GetPaginatedJobsInDetailForCurrentOrClosedPeriod(short jobStatus, int pageSize = int.MaxValue, int pageNumber = 1);

        Task<DasMismatchModel> GetJobsThatAreADasMismatchAsync(int? collectionYear, CancellationToken cancellationToken);

        Task<JobsFailedCurrentPeriodModel> GetFailedJobsInCurrentPeriodAsync(CancellationToken cancellationToken);

        Task<ProvidersReturnedCurrentPeriodModel> GetProvidersReturnedCurrentPeriodAsync(CancellationToken cancellationToken);

        Task<JobProcessingDetailModel> GetJobsProcessingCurrentPeriodAsync(short jobStatus, DateTime dateTime, CancellationToken cancellationToken);

        Task<JobProcessingDetailModel> GetJobsProcessingCurrentPeriodLast5MinsAsync(short jobStatus, DateTime dateTime, CancellationToken cancellationToken);

        Task<JobProcessingDetailModel> GetJobsProcessingCurrentPeriodLastHourAsync(short jobStatus, DateTime dateTime, CancellationToken cancellationToken);
    }
}