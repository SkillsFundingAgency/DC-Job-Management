using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.Controllers
{
    public abstract class AbstractPathController
    {
        private const int MaxNumberOfPathItems = 10;

        private readonly IOrderedEnumerable<IPathItem> _pathItems;
        private readonly IStateService _stateService;
        private readonly IPathItemParamsFactory _paramsFactory;
        private readonly IValidityPeriodService _validityPeriodService;
        private readonly IPeriodEndRepository _periodEndRepository;
        private readonly ILogger _logger;

        protected AbstractPathController(
            IStateService stateService,
            IOrderedEnumerable<IPathItem> pathItems,
            IPathItemParamsFactory paramsFactory,
            IValidityPeriodService validityPeriodService,
            IPeriodEndRepository periodEndRepository,
            ILogger logger)
        {
            _stateService = stateService;
            _pathItems = pathItems;
            _paramsFactory = paramsFactory;
            _validityPeriodService = validityPeriodService;
            _periodEndRepository = periodEndRepository;
            _logger = logger;
        }

        public abstract int PathId { get; }

        public abstract string CollectionType { get; }

        public virtual int EntityType => (int)PeriodEndEntityType.Path;

        public bool IsMatch(int pathId)
        {
            return pathId == PathId;
        }

        public async Task<IEnumerable<int>> Execute(int collectionYear, int periodNumber)
        {
            if (await _stateService.SavePathIsBusyAsync(PathId, true))
            {
                try
                {
                    PeriodEndJobState state = await _stateService.GetStateForPathId(PathId, collectionYear, periodNumber);

                    _logger.LogDebug($"Path {PathId} called for collection {state.CollectionName} for period {collectionYear}/R{periodNumber:D2}");

                    var runningJobs = await _stateService.GetPathItemJobStates(PathId, collectionYear, periodNumber);

                    if (runningJobs != null && runningJobs.Any(j => !PeriodEndConstants.JobFinished.Contains(j.Status)))
                    {
                        _logger.LogDebug($"Jobs are running for path {PathId}");
                        return null;
                    }

                    var subPaths = await ProcessNextPathItem(collectionYear, state);
                    return subPaths;
                }
                finally
                {
                    await _stateService.SavePathIsBusyAsync(PathId, false);
                }
            }

            return null;
        }

        public async Task<IEnumerable<PathItemModel>> GetPathItems(
            int collectionYear,
            int periodNumber,
            IDictionary<int, PathItemJobsWithSummaries> pathItemJobsWithSummariesForPaths,
            IDictionary<string, IEnumerable<int>> validities,
            IDictionary<int, bool> pathItemExpectsJobs,
            bool validity,
            CancellationToken cancellationToken)
        {
            var items = new List<PathItemModel>();

            var ordinal = 0;
            foreach (var pathItem in _pathItems)
            {
                var subPathValid = true;

                if (validities != null
                    && !await pathItem.IsValidForPeriod(periodNumber, collectionYear, validities)
                    && HasNoSubPathItems(pathItem))
                {
                    continue;
                }

                if (pathItem.ItemSubPaths != null)
                {
                    subPathValid = await _periodEndRepository.GetValidityForSubPathAsync(pathItem.ItemSubPaths.First(), collectionYear, periodNumber, cancellationToken);
                    if (!subPathValid && pathItem is AbstractHiddenPathItem)
                    {
                        continue;
                    }
                }

                var item = new PathItemModel
                {
                    PathId = PathId,
                    PathItemId = pathItem.PathItemId,
                    Name = pathItem.DisplayName,
                    Ordinal = ordinal,
                    EntityType = pathItem.EntityType,
                    IsProviderReport = pathItem is IProviderReport,
                    SubPaths = validity ? pathItem.ItemSubPaths : subPathValid ? pathItem.ItemSubPaths : null,
                    IsPausing = pathItem.IsPausing,
                    HasJobs = pathItemExpectsJobs.ContainsKey(ordinal) && pathItemExpectsJobs[ordinal],
                    Hidden = pathItem.IsHidden,
                    IsInitiating = pathItem.IsInitiating
                };

                if (pathItemJobsWithSummariesForPaths != null && pathItemJobsWithSummariesForPaths.TryGetValue(PathId, out var pathItemJobsWithSummaries))
                {
                    item.PathItemJobs = pathItemJobsWithSummaries.Jobs.Where(job => job.Ordinal == ordinal).ToList();

                    if (item.PathItemJobs.Count >= MaxNumberOfPathItems)
                    {
                        item.PathItemJobSummary = pathItemJobsWithSummaries.Summaries.SingleOrDefault(x => x.Ordinal == ordinal);
                    }

                    if (item.PathItemJobs.Count(j => JobTypeConstants.FailedStates.Contains(j.Status)) > MaxNumberOfPathItems)
                    {
                        item.DisplayAllJobs = true;
                    }
                }

                items.Add(item);

                ordinal++;
            }

            return items;
        }

        public async virtual Task<bool> IsValid(int year, int period)
        {
            return true;
        }

        private bool HasNoSubPathItems(IPathItem pathItem)
        {
            return pathItem.ItemSubPaths == null || pathItem.ItemSubPaths.Count == 0;
        }

        private async Task<IEnumerable<int>> ProcessNextPathItem(int collectionYear, PeriodEndJobState state)
        {
            var validities = await _validityPeriodService.GetValidPeriodsForCollections(collectionYear);

            var subPaths = new List<int>();

            var position = state.Position;
            _logger.LogDebug($"Path {PathId} is in position {state.Position}");

            var validPathItems = new List<IPathItem>();
            foreach (var pathItem in _pathItems)
            {
                if (!await pathItem.IsValidForPeriod(state.Period, collectionYear, validities))
                {
                    _logger.LogDebug($"Item {pathItem.DisplayName} is not set to valid for period {state.Period}");
                    continue;
                }

                validPathItems.Add(pathItem);
            }

            foreach (var item in validPathItems.Skip(position))
            {
                var pathItemParams = _paramsFactory.GetPathItemParams(position, state.Year ?? 0, state.Period, state.CollectionName, PathId);
                var itemReturn = await item.ExecuteAsync(pathItemParams);

                if (itemReturn.SubPaths != null && itemReturn.SubPaths.Any())
                {
                    subPaths.AddRange(itemReturn.SubPaths);
                }

                if (itemReturn.BlockingTask)
                {
                    _logger.LogDebug($"{item.DisplayName} is blocking, so breaking out of path {PathId}.");
                    break;
                }

                position++;
            }

            return subPaths;
        }
    }
}