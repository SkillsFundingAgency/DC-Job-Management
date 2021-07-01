using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public abstract class AbstractPeriodEndService
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IPathController> _pathControllers;
        private readonly IStateService _stateService;
        private readonly IPeriodEndRepository _repository;
        private readonly IQueryService _queryService;
        private readonly IReturnCalendarService _returnCalendarService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPathStructureService _pathStructureService;
        private readonly IFailedJobQueryService _failedJobQueryService;

        protected AbstractPeriodEndService(
            ILogger logger,
            IEnumerable<IPathController> pathControllers,
            IStateService stateService,
            IPeriodEndRepository repository,
            IQueryService queryService,
            IReturnCalendarService returnCalendarService,
            IFailedJobQueryService failedJobQueryService,
            IDateTimeProvider dateTimeProvider,
            IPathStructureService pathStructureService)
        {
            _logger = logger;
            _pathControllers = pathControllers;
            _stateService = stateService;
            _repository = repository;
            _queryService = queryService;
            _returnCalendarService = returnCalendarService;
            _dateTimeProvider = dateTimeProvider;
            _pathStructureService = pathStructureService;
            _failedJobQueryService = failedJobQueryService;
        }

        public async Task ProceedAsync(int collectionYear, int period, int pathId, CancellationToken cancellationToken)
        {
            await ProceedAsync(new List<int> { pathId }, collectionYear, period, cancellationToken);
        }

        public async Task ProceedAsync(long jobId = 0, CancellationToken cancellationToken = default(CancellationToken))
        {
            var pathYearPeriod = await _stateService.GetPathforJob(jobId);

            await ProceedAsync(new List<int> { pathYearPeriod.HubPathId }, pathYearPeriod.Year ?? 0, pathYearPeriod.Period, cancellationToken);
        }

        public async Task ProceedAsync(IEnumerable<int> pathIds, int year, int period, CancellationToken cancellationToken)
        {
            foreach (var pathId in pathIds)
            {
                var controller = _pathControllers.FirstOrDefault(pc => pc.IsMatch(pathId));

                if (controller == null)
                {
                    _logger.LogWarning($"Path Id {pathId} does not match a path.");
                    continue;
                }

                if (!await controller.IsValid(year, period))
                {
                    continue;
                }

                var subPaths = (await controller.Execute(year, period))?.ToList();

                if (subPaths != null && subPaths.Any())
                {
                    await ProceedAsync(subPaths, year, period, cancellationToken);
                }
            }
        }

        public async Task<PeriodEndStateModel> GetPathsStateAsync(int? collectionYear, int? periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            var period = await GetPeriodEndPeriodAsync(collectionYear, periodNumber, collectionType, cancellationToken);
            if (period == null)
            {
                return null;
            }

            return await _pathStructureService.GetPathsStateAsync(period, collectionType, false, cancellationToken);
        }

        public virtual async Task InitialisePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken)
        {
            var collectionName = collectionType + year;

            var paths = _pathControllers.Where(w => w.CollectionType == collectionType).ToDictionary(x => x.PathId, x => x.Name);

            await _queryService.ClearDownPeriodEnd(collectionName, period);

            await _repository.InitialisePeriodEndAsync(period, collectionName, paths);
        }

        public virtual async Task StartPeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _repository.StartPeriodEndAsync(year, period, collectionType);

            var subPaths = (await _pathControllers.Single(pc => pc.IsMatch(Convert.ToInt32(PeriodEndPath.ILRCriticalPath)) && pc.CollectionType == collectionType).Execute(year, period))?.ToList();

            if (subPaths != null && subPaths.Any())
            {
                await ProceedAsync(subPaths, year, period, cancellationToken);
            }
        }

        public async Task CollectionClosedEmailSentAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            await _repository.CollectionClosedEmailSentAsync(collectionYear, periodNumber, collectionType);
        }

        public virtual async Task ClosePeriodEndAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            await _repository.ClosePeriodEndAsync(collectionYear, periodNumber, collectionType);
        }

        public virtual async Task<PeriodEndPrepModel> GetPrepStateAsync(int? collectionYear, int? periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            var period = await GetPeriodEndPeriodAsync(collectionYear, periodNumber, collectionType, cancellationToken);
            if (period == null)
            {
                return null;
            }

            IEnumerable<JobSchedule> refDataJobs = Enumerable.Empty<JobSchedule>();
            IEnumerable<McaDetails> mcaDetails = Enumerable.Empty<McaDetails>();

            var failedJobs = _failedJobQueryService.GetFailedJobsPerPeriod(period.Year, period.Period);
            var parentStateModelTask = _stateService.GetStateAsync(period.Year, period.Period, collectionType);

            await Task.WhenAll(failedJobs, parentStateModelTask);

            var parentStateModel = parentStateModelTask.Result ?? new PeriodEndStateModel();

            parentStateModel.CollectionClosed = period.PeriodClosed;

            return new PeriodEndPrepModel
            {
                ReferenceDataJobs = refDataJobs.ToList(),
                FailedJobs = failedJobs.Result.ToList(),
                State = parentStateModel,
                McaDetails = mcaDetails.ToList()
            };
        }

        public async Task<YearPeriod> GetPeriodEndPeriodAsync(int? collectionYear, int? periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            DateTime dateTimeUtc = _dateTimeProvider.GetNowUtc();

            if (collectionYear == null || periodNumber == null)
            {
                var periodEnd = await _returnCalendarService.GetPeriodEndPeriod(collectionType);

                if (periodEnd?.Year == null)
                {
                    return null;
                }

                return periodEnd;
            }
            else
            {
                ReturnPeriod returnPeriod = await _returnCalendarService.GetPeriodForCollectionType(collectionType, collectionYear.Value, periodNumber.Value);

                return new YearPeriod
                {
                    Period = periodNumber.Value,
                    PeriodClosed = returnPeriod.EndDateTimeUtc < dateTimeUtc,
                    PeriodEndDate = returnPeriod.EndDateTimeUtc,
                    Year = collectionYear.Value
                };
            }
        }
    }
}