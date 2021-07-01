using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Interfaces.PathItems
{
    public interface IPathItem
    {
        string DisplayName { get; }

        string ReportFileName { get; }

        int PathItemId { get; }

        bool IsPausing { get; }

        bool IsHidden { get; }

        bool IsInitiating { get; }

        PeriodEndEntityType EntityType { get; }

        List<int> ItemSubPaths { get; }

        string CollectionNameInDatabase { get; }

        Task<bool> IsValidForPeriod(
            int period,
            int collectionYear,
            IDictionary<string, IEnumerable<int>> validities);

        Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams);
    }
}