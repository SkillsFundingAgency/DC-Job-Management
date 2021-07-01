using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;

namespace ESFA.DC.PeriodEnd.Services.Controllers
{
    public abstract class AbstractSubPathController : AbstractPathController
    {
        private readonly IPeriodEndRepository _periodEndRepository;

        protected AbstractSubPathController(
            IStateService stateService,
            IOrderedEnumerable<IPathItem> pathItems,
            IPathItemParamsFactory paramsFactory,
            IValidityPeriodService validityPeriodService,
            ILogger logger,
            IPeriodEndRepository periodEndRepository)
            : base(stateService, pathItems, paramsFactory, validityPeriodService, periodEndRepository, logger)
        {
            _periodEndRepository = periodEndRepository;
        }

        public override async Task<bool> IsValid(int year, int period)
        {
            return await _periodEndRepository.GetValidityForSubPathAsync(PathId, year, period, CancellationToken.None);
        }
    }
}