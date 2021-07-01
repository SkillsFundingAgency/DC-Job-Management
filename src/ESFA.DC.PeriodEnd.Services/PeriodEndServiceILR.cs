using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.DataAccess;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public class PeriodEndServiceILR : AbstractPeriodEndService, IPeriodEndServiceILR
    {
        private readonly IPeriodEndRepository _repository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IDASStoppingPathItem _dasEnd;
        private readonly IPathItemParamsFactory _paramsFactory;
        private readonly IValidityPeriodService _validityPeriodService;
        private readonly IJobQueryService _jobQueryService;
        private readonly IDASPaymentsService _dasPaymentsService;

        public PeriodEndServiceILR(
            ILogger logger,
            IEnumerable<IPathController> pathControllers,
            IStateService stateService,
            IPeriodEndRepository repository,
            IQueryService queryService,
            IScheduleRepository scheduleRepository,
            IDASStoppingPathItem dasEnd,
            IPathItemParamsFactory paramsFactory,
            IValidityPeriodService validityPeriodService,
            IReturnCalendarService returnCalendarService,
            IFailedJobQueryServiceILR failedJobQueryService,
            IDateTimeProvider dateTimeProvider,
            IPathStructureServiceILR pathStructureService,
            IDASPaymentsService dasPaymentsService,
            IJobQueryService jobQueryService)
            : base(logger, pathControllers, stateService, repository, queryService, returnCalendarService, failedJobQueryService, dateTimeProvider, pathStructureService)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
            _dasEnd = dasEnd;
            _paramsFactory = paramsFactory;
            _validityPeriodService = validityPeriodService;
            _jobQueryService = jobQueryService;
            _dasPaymentsService = dasPaymentsService;
        }

        public async Task ToggleReferenceDataJobsAsync(bool pause, CancellationToken cancellationToken)
        {
            await _repository.ToggleReferenceDataJobsAsync(pause);
        }

        public new async Task<bool> ClosePeriodEndAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            await base.ClosePeriodEndAsync(collectionYear, periodNumber, collectionType, cancellationToken);

            var validities = await _validityPeriodService.GetValidPeriodsForCollections(collectionYear);
            if (await _dasEnd.IsValidForPeriod(periodNumber, collectionYear, validities))
            {
                var pathItemParams = _paramsFactory.GetPathItemParams(
                    int.MaxValue,
                    collectionYear,
                    periodNumber,
                    PeriodEndConstants.CollectionName_DasEnd,
                    (int)PeriodEndPath.ILRCriticalPath);

                await _dasEnd.ExecuteAsync(pathItemParams);
            }

            var state = await GetPathsStateAsync(collectionYear, periodNumber, collectionType, cancellationToken);

            return state.ReferenceDataJobsPaused;
        }

        public async Task<List<ReportsPublished>> GetPublishedReportPeriodsAsync(CancellationToken cancellationToken)
        {
            return (await _repository.GetPublishedReportPeriodsAsync()).ToList();
        }

        public override async Task<PeriodEndPrepModel> GetPrepStateAsync(int? collectionYear, int? periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            var period = await GetPeriodEndPeriodAsync(collectionYear, periodNumber, collectionType, cancellationToken);
            if (period == null)
            {
                return null;
            }

            var prepModel = await base.GetPrepStateAsync(period.Year, period.Period, collectionType, cancellationToken);

            var refDataJobs = await GetReferenceDataJobsAsync(cancellationToken);
            var mcaDetails = await _repository.GetActiveMcaProvidersAsync(period.Year, period.PeriodEndDate, cancellationToken);

            prepModel.ReferenceDataJobs = refDataJobs;
            prepModel.McaDetails = mcaDetails;

            var successfulJobs = (await _jobQueryService
                .GetAllSuccessfulJobsPerCollectionTypePerPeriodAsync(period.Year, period.Period, collectionType, CancellationToken.None))
                .Select(j => new CurrentPeriodSuccessfulIlrModel
                {
                    JobId = j.JobId,
                    Ukprn = j.Ukprn,
                    PeriodNumber = period.Period,
                    CollectionYear = period.Year
                });

            prepModel.SLDDASMismatches = (await _dasPaymentsService.GetMissingDASPayments(successfulJobs, period.Year, period.Period, CancellationToken.None)).Count();

            return prepModel;
        }

        public async Task<List<JobSchedule>> GetReferenceDataJobsAsync(CancellationToken cancellationToken)
        {
            return await _scheduleRepository.GetJobSchedules(PeriodEndConstants.ReferenceDataJobsToHold);
        }
    }
}