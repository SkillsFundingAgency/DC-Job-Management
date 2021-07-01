using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Interfaces.DataAccess
{
    public interface IPeriodEndRepository
    {
        Task<PeriodEndJobState> GetStateForPathIdAsync(int pathId, int collectionYear, int periodNumber, CancellationToken cancellationToken = default(CancellationToken));

        Task<PathYearPeriod> GetPathforJobAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task SavePathItemAsync(PathItemModel pathItemModel, int collectionYear, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<PeriodEndState> GetStateAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<PathItemJobModel>> GetPathItemJobStatesAsync(int pathId, int collectionYear, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<Dictionary<int, PathItemJobsWithSummaries>> GetPathItemJobStatesWithSummaryAsync(int collectionYear, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<PathModel>> GetPathsForPeriodAsync(
            int collectionYear,
            int periodNumber,
            string collectionType,
            CancellationToken cancellationToken);

        Task<IEnumerable<PathItemModel>> GetPathItemsForPathAsync(int pathId, int collectionYear, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<int>> GetValidityPeriodsForEmailAsync(int hubEmailId, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> GetValidityForSubPathAsync(int pathId, int year, int period, CancellationToken cancellationToken);

        Task<IEnumerable<PeriodEndJobState>> GetStateForPathsAsync(int collectionYear, int periodNumber, CancellationToken cancellationToken = default(CancellationToken));

        Task InitialisePeriodEndAsync(int period, string collectionName, IDictionary<int, string> paths, CancellationToken cancellationToken = default(CancellationToken));

        Task StartPeriodEndAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task ToggleReferenceDataJobsAsync(bool pause, CancellationToken cancellationToken = default(CancellationToken));

        Task CollectionClosedEmailSentAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task ProviderReportsReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishProviderReportsAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task Fm36ReportsReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishFm36ReportsAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task McaReportsReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishMcaReportsAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task ClosePeriodEndAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportsPublished>> GetPublishedReportPeriodsAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<List<McaDetails>> GetActiveMcaProvidersAsync(int collectionYear, DateTime periodEndDate, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<HistoryDetail>> GetPeriodEndHistoryDetailsAsync(string collectionName, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<int>> GetCollectionYearsAsync(string collectionNamePrefix, CancellationToken cancellationToken = default(CancellationToken));

        Task<IDictionary<string, IEnumerable<int>>> GetValidPeriodsForCollectionsAndEmailsAsync(int collectionYear, CancellationToken cancellationToken = default(CancellationToken));

        Task EsfSummarisationReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task DcSummarisationReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task AppsSummarisationReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> GetPeriodEndJobsForWeekendDaysAsync(int periodEndId, List<DateTime> weekendDays, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> SavePathIsBusyAsync(int pathId, bool isBusy, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> HasPeriodEndRunForPeriodAsync(string collectionType, int collectionYear, int period, CancellationToken cancellationToken);

        Task<bool> IsPeriodEndRunning(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken);

        Task<Dictionary<int, bool>> PathItemsHaveJobsAsync(int pathId, int period, int collectionYear, CancellationToken cancellationToken);
    }
}