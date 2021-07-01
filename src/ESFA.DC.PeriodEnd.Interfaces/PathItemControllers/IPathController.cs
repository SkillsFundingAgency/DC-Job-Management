using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Interfaces.PathItemControllers
{
    public interface IPathController
    {
        string Name { get; }

        int PathId { get; }

        string CollectionType { get; }

        int EntityType { get; }

        bool IsMatch(int listId);

        Task<bool> IsValid(int year, int period);

        Task<IEnumerable<int>> Execute(int year, int period);

        Task<IEnumerable<PathItemModel>> GetPathItems(
            int collectionYear,
            int periodNumber,
            IDictionary<int, PathItemJobsWithSummaries> pathItemJobsWithSummariesForPaths,
            IDictionary<string, IEnumerable<int>> validities,
            IDictionary<int, bool> pathItemExpectsJobs,
            bool validity,
            CancellationToken cancellationToken);
    }
}