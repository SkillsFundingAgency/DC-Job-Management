using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public class PeriodEndServiceALLF : AbstractPeriodEndService, IPeriodEndServiceALLF
    {
        private const string CollectionType = "Generic";

        private readonly IEnumerable<IPathControllerALLF> _pathControllers;
        private readonly IStateService _stateService;
        private readonly IPeriodEndRepository _repository;
        private readonly ICollectionService _collectionService;
        private readonly IQueryService _queryService;
        private readonly IJobQueryService _jobQueryService;

        public PeriodEndServiceALLF(
            ILogger logger,
            IEnumerable<IPathControllerALLF> pathControllers,
            IStateService stateService,
            IPeriodEndRepository repository,
            ICollectionService collectionService,
            IQueryService queryService,
            IJobQueryService jobQueryService,
            IReturnCalendarService returnCalendarService,
            IDateTimeProvider dateTimeProvider,
            IPathStructureServiceALLF pathStructureService)
            : base(logger, (IEnumerable<IPathController>)pathControllers, stateService, repository, queryService, returnCalendarService, null, dateTimeProvider, pathStructureService)
        {
            _pathControllers = pathControllers;
            _stateService = stateService;
            _repository = repository;
            _collectionService = collectionService;
            _queryService = queryService;
            _jobQueryService = jobQueryService;
        }

        public async Task<IEnumerable<JobMetaDataDto>> GetFileUploadsForPeriodAsync(
            int collectionYear,
            int period,
            CancellationToken cancellationToken)
        {
            return await _jobQueryService.GetAllJobsPerCollectionPerPeriodAsync(collectionYear, period, CollectionType, cancellationToken);
        }

        public override async Task InitialisePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken)
        {
            var collection = await _collectionService.GetCollectionAsync(CancellationToken.None, collectionType);
            var paths = _pathControllers.Where(w => w.CollectionType == collectionType).ToDictionary(x => x.PathId, x => x.Name);

            await _queryService.ClearDownPeriodEnd(collection.CollectionTitle, period);

            await _repository.InitialisePeriodEndAsync(period, collection.CollectionTitle, paths, cancellationToken);
        }

        public override async Task StartPeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _repository.StartPeriodEndAsync(year, period, collectionType, cancellationToken);

            var subPaths = (await _pathControllers.Single(pc => pc.IsMatch(Convert.ToInt32(PeriodEndPath.ALLFCriticalPath)) && pc.CollectionType == collectionType).Execute(year, period))?.ToList();

            if (subPaths != null && subPaths.Any())
            {
                await ProceedAsync(subPaths, year, period, cancellationToken);
            }
        }

        public override async Task<PeriodEndPrepModel> GetPrepStateAsync(int? collectionYear, int? periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            var period = await GetPeriodEndPeriodAsync(collectionYear, periodNumber, collectionType, cancellationToken);
            if (period == null)
            {
                return null;
            }

            var parentStateModel = await _stateService.GetStateAsync(period.Year, period.Period, collectionType);

            parentStateModel.CollectionClosed = period.PeriodClosed;

            return new PeriodEndPrepModel
            {
                State = parentStateModel
            };
        }
    }
}