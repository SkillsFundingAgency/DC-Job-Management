using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Services.Persistence
{
    public class StateService : IStateService
    {
        private readonly IPeriodEndRepository _periodEndRepository;

        public StateService(IPeriodEndRepository periodEndRepository)
        {
            _periodEndRepository = periodEndRepository;
        }

        public async Task<PeriodEndState> GetStateAsync(int collectionYear, int periodNumber, string collectionType)
        {
            return await _periodEndRepository.GetStateAsync(collectionYear, periodNumber, collectionType);
        }

        public async Task<PeriodEndJobState> GetStateForPathId(int pathId, int collectionYear, int periodNumber)
        {
            return await _periodEndRepository.GetStateForPathIdAsync(pathId, collectionYear, periodNumber);
        }

        public async Task<IEnumerable<PeriodEndJobState>> GetStateForPaths(int collectionYear, int periodNumber)
        {
            return await _periodEndRepository.GetStateForPathsAsync(collectionYear, periodNumber);
        }

        public async Task<PathYearPeriod> GetPathforJob(long jobId)
        {
            return await _periodEndRepository.GetPathforJobAsync(jobId);
        }

        public async Task SavePathItem(PathItemModel pathItem, int collectionYear, int period)
        {
            await _periodEndRepository.SavePathItemAsync(pathItem, collectionYear, period);
        }

        public async Task<bool> SavePathIsBusyAsync(int pathId, bool isBusy)
        {
            return await _periodEndRepository.SavePathIsBusyAsync(pathId, isBusy);
        }

        public async Task<IEnumerable<PathItemModel>> GetPathItemsForPath(int pathId, int collectionYear, int periodNumber)
        {
            return await _periodEndRepository.GetPathItemsForPathAsync(pathId, collectionYear, periodNumber);
        }

        public async Task<Dictionary<int, PathItemJobsWithSummaries>> GetPathItemJobStatesWithSummary(int collectionYear, int period)
        {
            return await _periodEndRepository.GetPathItemJobStatesWithSummaryAsync(collectionYear, period);
        }

        public async Task<IEnumerable<PathItemJobModel>> GetPathItemJobStates(int pathId, int collectionYear, int period)
        {
            return await _periodEndRepository.GetPathItemJobStatesAsync(pathId, collectionYear, period);
        }

        public async Task<Dictionary<int, bool>> PathItemsHaveJobsAsync(int pathId, int period, int collectionYear, CancellationToken cancellationToken)
        {
            return await _periodEndRepository.PathItemsHaveJobsAsync(pathId, period, collectionYear, cancellationToken);
        }
    }
}