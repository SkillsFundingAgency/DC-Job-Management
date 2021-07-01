using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Services.Controllers;

namespace ESFA.DC.PeriodEnd.Services.ALLF.Controllers
{
    public abstract class AbstractPathControllerALLF : AbstractPathController
    {
        protected AbstractPathControllerALLF(
            IStateService stateService,
            IOrderedEnumerable<IALLFPathItem> pathItems,
            IPathItemParamsFactory paramsFactory,
            IValidityPeriodService validityPeriodService,
            IPeriodEndRepository periodEndRepository,
            ILogger logger)
            : base(stateService, (IOrderedEnumerable<IPathItem>)pathItems, paramsFactory, validityPeriodService, periodEndRepository, logger)
        {
        }

        public async virtual Task<bool> IsValid(int year, int period)
        {
            return true;
        }
    }
}
