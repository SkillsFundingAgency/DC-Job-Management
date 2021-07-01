using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IReturnCalendarService
    {
        Task<ReturnPeriod> GetCurrentPeriodAsync(string collectionName);

        Task<ReturnPeriod> GetNextPeriodAsync(string collectionName);

        Task<ReturnPeriod> GetNextPeriodAsync(int collectionId);

        Task<ReturnPeriod> GetPeriodAsync(string collectionName, DateTime dateTimeUTC);

        Task<ReturnPeriod> GetPeriodAsync(int collectionId, int periodNumber);

        Task<ReturnPeriod> GetPreviousPeriodAsync(string collectionName, DateTime dateTimeUtc);

        Task<ReturnPeriod> GetPreviousPeriodAsync(string collectionName, CancellationToken cancellationToken = default(CancellationToken));

        Task<ReturnPeriod> GetPeriodAsync(int collectionId, DateTime dateTimeUtc);

        Task<List<ReturnPeriod>> GetOpenPeriodsAsync(DateTime? dateTimeUtc = null, string collectionType = CollectionTypeConstants.Ilr);

        Task<ReturnPeriod> GetRecentlyClosedPeriodAsync(DateTime? dateTimeUtc = null, string collectionType = CollectionTypeConstants.Ilr);

        Task<ReturnPeriod[]> GetAllPeriodsAsync(string collectionName = null, string collectionType = null);

        Task<YearPeriod> GetPeriodEndPeriod(string collectionType);

        Task<ReturnPeriod> GetPeriodForCollectionType(string collectionType, int collectionYear, int periodNumber);

        Task<bool> IsReferenceDataCollectionExpiredAsync(string collectionName, CancellationToken cancellationToken);

        Task<ReturnPeriod> GetNextClosingPeriodAsync(string collectionType, CancellationToken cancellationToken);
    }
}
