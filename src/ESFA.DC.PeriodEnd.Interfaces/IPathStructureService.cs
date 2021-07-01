using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IPathStructureService
    {
        Task<PeriodEndStateModel> GetPathsStateAsync(YearPeriod period, string collectionType, bool validity, CancellationToken cancellationToken);

        Task<List<PathPathItemsModel>> GetPathStructures(
            YearPeriod period,
            IEnumerable<PeriodEndJobState> stateModels,
            IDictionary<int, PathItemJobsWithSummaries> pathItemJobsWithSummaries,
            IDictionary<string, IEnumerable<int>> validities,
            bool allPaths,
            bool validity,
            CancellationToken cancellationToken);
    }
}