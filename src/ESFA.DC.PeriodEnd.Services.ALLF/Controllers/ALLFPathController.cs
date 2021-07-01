using System;
using System.Linq;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ALLF.Controllers
{
    public class ALLFPathController : AbstractPathControllerALLF, IPathControllerALLF
    {
        public ALLFPathController(
            IStateService stateService,
            IOrderedEnumerable<IALLFPathItem> pathItems,
            IPathItemParamsFactory pathItemParamsFactory,
            IValidityPeriodService validityPeriodService,
            IPeriodEndRepository periodEndRepository,
            ILogger logger)
            : base(stateService, pathItems, pathItemParamsFactory, validityPeriodService, periodEndRepository, logger)
        {
        }

        public string Name => "Critical Path";

        public override int PathId => Convert.ToInt32(PeriodEndPath.ALLFCriticalPath);

        public override string CollectionType => PeriodEndConstants.GenericCollectionType;
    }
}