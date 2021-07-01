using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public abstract class AbstractHistoryService : IHistoryService
    {
        private readonly IStateService _stateService;
        private readonly IPeriodEndRepository _periodEndRepository;
        private readonly IPeriodEndDateTimeService _periodEndDateTimeService;

        protected AbstractHistoryService(
            IStateService stateService,
            IPeriodEndRepository periodEndRepository,
            IPeriodEndDateTimeService periodEndDateTimeService)
        {
            _stateService = stateService;
            _periodEndRepository = periodEndRepository;
            _periodEndDateTimeService = periodEndDateTimeService;
        }

        public abstract string CollectionNamePrefix { get; }

        public async Task<IEnumerable<HistoryDetail>> GetPeriodHistories(int collectionYear)
        {
            var collectionName = CollectionNamePrefix + collectionYear;

            var result = await _periodEndRepository.GetPeriodEndHistoryDetailsAsync(collectionName);

            foreach (var c in result)
            {
                c.PeriodEndRuntimeDays = _periodEndDateTimeService.CalculateRuntimeSimple(c.PeriodEndStart, c.PeriodEndFinish);
                var serviceValues = await _periodEndDateTimeService.CalculateRuntimeBusiness(c.PeriodEndStart, c.PeriodEndFinish);
                var weekendDays = await _periodEndRepository.GetPeriodEndJobsForWeekendDaysAsync(c.PeriodEndId, serviceValues.WeekendDays);
                var weekendDaysTimeSpan = new TimeSpan(weekendDays, 0, 0, 0);
                c.PeriodEndRuntimeBusinessDays = serviceValues.TimeDifference.Add(weekendDaysTimeSpan);
            }

            return result;
        }

        public async Task<PeriodEndStateModel> GetPathsHistoryStateAsync(YearPeriod period, string collectionType, CancellationToken cancellationToken)
        {
            var stateModel = new PeriodEndStateModel
            {
                PeriodEndStarted = true,
                PeriodEndFinished = true,
                CollectionClosed = true,
                CollectionClosedEmailSent = true,
                ReferenceDataJobsPaused = true,
                McaReportsPublished = true,
                McaReportsReady = true,
                ProviderReportsPublished = true,
                ProviderReportsReady = true,
                Fm36ReportsPublished = true,
                Fm36ReportsReady = true
            };

            var paths = await _periodEndRepository.GetPathsForPeriodAsync(period.Year, period.Period, collectionType, cancellationToken);

            var pathItemJobsWithSummaries = await _stateService.GetPathItemJobStatesWithSummary(period.Year, period.Period);

            var pathModels = new List<PathPathItemsModel>();
            foreach (var path in paths)
            {
                var pathItems = (await _periodEndRepository.GetPathItemsForPathAsync(path.PathId, period.Year, period.Period, cancellationToken)).ToList();
                pathItems = pathItems.Select(pi =>
                {
                    if (!pathItemJobsWithSummaries.ContainsKey(path.HubPathId))
                    {
                        return pi;
                    }

                    var foundJobs = pi.PathItemJobs;

                    pi.PathItemJobs = pathItemJobsWithSummaries[path.HubPathId].Jobs
                        .Where(pij => foundJobs.Any(fj => fj.JobId == pij.JobId))
                        .Take(10)
                        .ToList();

                    if (pi.PathItemJobs.Count > 9)
                    {
                        pi.PathItemJobSummary = pathItemJobsWithSummaries[path.HubPathId].Summaries
                            .FirstOrDefault(s => s.PathItemLabel == pi.Name);
                    }

                    return pi;
                }).ToList();

                pathModels.Add(new PathPathItemsModel
                {
                    Name = path.PathLabel,
                    EntityType = (int)PeriodEndEntityType.Path,
                    IsCritical = PeriodEndConstants.CriticalPaths.Contains(path.HubPathId),
                    IsBusy = false,
                    IsPreviousPeriod = true,
                    PathId = path.HubPathId,
                    Position = pathItems.Count,
                    PathItems = pathItems
                });
            }

            stateModel.Paths = pathModels;
            return stateModel;
        }

        public async Task<IEnumerable<int>> GetCollectionYears()
        {
            var collectionName = CollectionNamePrefix;

            return await _periodEndRepository.GetCollectionYearsAsync(collectionName);
        }
    }
}