using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IReturnPeriodService
    {
        Task<IEnumerable<ReturnPeriod>> GetReturnPeriodsForCollectionAsync(int collectionId);

        Task<ReturnPeriod> GetReturnPeriodAsync(int id);

        Task<DateTime> GetPeriodEndEndDate(string collectionName, int periodNumber);

        Task<bool> UpdateReturnPeriod(ReturnPeriod returnPeriod, CancellationToken cancellationToken);

        Task<IEnumerable<ReturnPeriod>> GetReturnPeriodsUpToGivenDateForCollectionTypeAsync(DateTime givenDateUtc, string collectionType, CancellationToken cancellationToken);
    }
}
