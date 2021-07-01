using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public abstract class AbstractPathStructureService : IPathStructureService
    {
        private readonly IEnumerable<IPathController> _controllers;
        private readonly IStateService _stateService;
        private readonly IValidityPeriodService _validityPeriodService;
        private readonly IPeriodEndOutputService _periodEndOutputService;
        private readonly IDateTimeProvider _dateTimeProvider;

        protected AbstractPathStructureService(
            IEnumerable<IPathController> controllers,
            IStateService stateService,
            IValidityPeriodService validityPeriodService,
            IPeriodEndOutputService periodEndOutputService,
            IDateTimeProvider dateTimeProvider)
        {
            _controllers = controllers;
            _stateService = stateService;
            _validityPeriodService = validityPeriodService;
            _periodEndOutputService = periodEndOutputService;
            _dateTimeProvider = dateTimeProvider;
        }

        protected abstract int CriticalPathId { get; }

        public async Task<PeriodEndStateModel> GetPathsStateAsync(YearPeriod period, string collectionType, bool validity, CancellationToken cancellationToken)
        {
            var providerReportPaths = new List<PathPathItemsModel>();

            Task<PeriodEndState> parentStateModelTask = _stateService.GetStateAsync(period.Year, period.Period, collectionType);
            Task<Dictionary<int, PathItemJobsWithSummaries>> pathItemJobsWithSummariesTask = _stateService.GetPathItemJobStatesWithSummary(period.Year, period.Period);
            Task<IEnumerable<PeriodEndJobState>> stateModelsTask = _stateService.GetStateForPaths(period.Year, period.Period);
            Task<IDictionary<string, IEnumerable<int>>> validitiesTask = _validityPeriodService.GetValidPeriodsForCollections(period.Year);

            await Task.WhenAll(parentStateModelTask, pathItemJobsWithSummariesTask, stateModelsTask, validitiesTask);

            PeriodEndState parentStateModel = parentStateModelTask.Result ?? new PeriodEndState();
            parentStateModel.TimeStamp = _dateTimeProvider.ConvertUtcToUk(_dateTimeProvider.GetNowUtc());

            List<PeriodEndJobState> stateModels = stateModelsTask.Result.ToList();

            var paths = await GetPathStructures(period, stateModels, pathItemJobsWithSummariesTask.Result, validitiesTask.Result, false, validity, cancellationToken);

            var criticalPath = paths.Single(p => p.PathId == CriticalPathId);
            await _periodEndOutputService.CheckCriticalPath(parentStateModel, criticalPath, period.Year, period.Period, collectionType);

            if (!parentStateModel.ProviderReportsReady)
            {
                providerReportPaths = paths.Where(p => p.PathItems.Any(pi => pi.IsProviderReport)).ToList();
            }

            await _periodEndOutputService.CheckProviderReportJobs(providerReportPaths, period.Year, period.Period, collectionType);

            parentStateModel.CollectionClosed = period.PeriodClosed;
            parentStateModel.Paths = paths;

            return parentStateModel;
        }

        public async Task<List<PathPathItemsModel>> GetPathStructures(
            YearPeriod period,
            IEnumerable<PeriodEndJobState> stateModels,
            IDictionary<int, PathItemJobsWithSummaries> pathItemJobsWithSummaries,
            IDictionary<string, IEnumerable<int>> validities,
            bool allPaths,
            bool validity,
            CancellationToken cancellationToken)
        {
            var paths = new List<PathPathItemsModel>();

            foreach (var path in _controllers)
            {
                var checkItemsShouldHaveJobs = await _stateService.PathItemsHaveJobsAsync(path.PathId, period.Period, period.Year, cancellationToken);

                var stateModel = stateModels?.SingleOrDefault(x => x.PathId == path.PathId);

                var valid = await path.IsValid(period.Year, period.Period);

                if (!valid && !allPaths)
                {
                    continue;
                }

                var pathModel = new PathPathItemsModel
                {
                    Name = path.Name,
                    PathId = path.PathId,
                    EntityType = path.EntityType,
                    IsValidForPeriod = await path.IsValid(period.Year, period.Period) ? PeriodEndValidityState.Checked : PeriodEndValidityState.Unchecked,
                    PathItems = await path.GetPathItems(period.Year, period.Period, pathItemJobsWithSummaries, validities, checkItemsShouldHaveJobs, validity, cancellationToken),
                    Position = stateModel?.Position ?? -1,
                    IsBusy = stateModel?.IsBusy ?? false
                };

                paths.Add(pathModel);
            }

            return paths;
        }
    }
}