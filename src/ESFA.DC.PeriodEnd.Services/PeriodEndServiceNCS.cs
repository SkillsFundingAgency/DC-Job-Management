using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public class PeriodEndServiceNCS : AbstractPeriodEndService, IPeriodEndServiceNCS
    {
        private readonly IEnumerable<IPathControllerNCS> _pathControllers;
        private readonly IPeriodEndRepository _repository;

        public PeriodEndServiceNCS(
            ILogger logger,
            IEnumerable<IPathControllerNCS> pathControllers,
            IStateService stateService,
            IPeriodEndRepository repository,
            IQueryService queryService,
            IReturnCalendarService returnCalendarService,
            IFailedJobQueryServiceNCS failedJobQueryService,
            IDateTimeProvider dateTimeProvider,
            IPathStructureServiceNCS pathStructureService)
            : base(logger, (IEnumerable<IPathController>)pathControllers, stateService, repository, queryService, returnCalendarService, failedJobQueryService, dateTimeProvider, pathStructureService)
        {
            _pathControllers = pathControllers;
            _repository = repository;
        }

        public override async Task StartPeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _repository.StartPeriodEndAsync(year, period, collectionType);

            var subPaths = (await _pathControllers.Single(pc => pc.IsMatch(Convert.ToInt32(PeriodEndPath.NCSCriticalPath)) && pc.CollectionType == collectionType).Execute(year, period))?.ToList();

            if (subPaths != null && subPaths.Any())
            {
                await ProceedAsync(subPaths, year, period, cancellationToken);
            }
        }
    }
}