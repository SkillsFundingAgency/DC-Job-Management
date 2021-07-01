using System.Linq;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Services.Controllers;

namespace ESFA.DC.PeriodEnd.Services.NCS.Controllers
{
    public abstract class AbstractPathControllerNCS : AbstractPathController
    {
        protected AbstractPathControllerNCS(
            IStateService stateService,
            IOrderedEnumerable<INCSPathItem> pathItems,
            IPathItemParamsFactory paramsFactory,
            IValidityPeriodService validityPeriodService,
            IPeriodEndRepository periodEndRepository,
            ILogger logger)
            : base(stateService, (IOrderedEnumerable<IPathItem>)pathItems, paramsFactory, validityPeriodService, periodEndRepository, logger)
        {
        }
    }
}
