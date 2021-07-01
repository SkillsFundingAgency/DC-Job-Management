using System;
using System.Linq;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Controllers
{
    public class CriticalPathController : AbstractPathControllerILR, ICriticalPathController
    {
        public CriticalPathController(
            IStateService stateService,
            IOrderedEnumerable<IILRPathItem> pathItems,
            IPathItemParamsFactory pathItemParamsFactory,
            IValidityPeriodService validityPeriodService,
            IPeriodEndRepository periodEndRepository,
            ILogger logger)
            : base(stateService, pathItems, pathItemParamsFactory, validityPeriodService, periodEndRepository, logger)
        {
        }

        public string Name => "Critical Path";

        public override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        public override string CollectionType => PeriodEndConstants.IlrCollectionNamePrefix;
    }
}