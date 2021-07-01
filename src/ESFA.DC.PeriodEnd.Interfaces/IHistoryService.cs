using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IHistoryService
    {
        Task<IEnumerable<HistoryDetail>> GetPeriodHistories(int collectionYear);

        Task<PeriodEndStateModel> GetPathsHistoryStateAsync(YearPeriod period, string collectionType, CancellationToken cancellationToken);

        Task<IEnumerable<int>> GetCollectionYears();
    }
}