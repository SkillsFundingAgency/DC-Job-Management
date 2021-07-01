using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IStateService
    {
        Task<PathYearPeriod> GetPathforJob(long jobId);

        Task SavePathItem(PathItemModel pathItem, int collectionYear, int period);

        Task<PeriodEndState> GetStateAsync(int collectionYear, int periodNumber, string collectionType);

        Task<PeriodEndJobState> GetStateForPathId(int pathId, int collectionYear, int periodNumber);

        Task<IEnumerable<PeriodEndJobState>> GetStateForPaths(int collectionYear, int periodNumber);

        Task<IEnumerable<PathItemModel>> GetPathItemsForPath(int pathId, int collectionYear, int periodNumber);

        Task<Dictionary<int, PathItemJobsWithSummaries>> GetPathItemJobStatesWithSummary(int collectionYear, int period);

        Task<IEnumerable<PathItemJobModel>> GetPathItemJobStates(int pathId, int collectionYear, int period);

        Task<bool> SavePathIsBusyAsync(int pathId, bool isBusy);

        Task<Dictionary<int, bool>> PathItemsHaveJobsAsync(int pathId, int period, int collectionYear, CancellationToken cancellationToken);
    }
}
