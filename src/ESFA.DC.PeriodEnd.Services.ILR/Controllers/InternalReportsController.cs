using System;
using System.Linq;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Services.Controllers;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Controllers
{
    public class InternalReportsController : AbstractSubPathController, IInternalReportsController
    {
        public InternalReportsController(
            IStateService stateService,
            IOrderedEnumerable<IInternalReportsPathItem> pathItems,
            IPathItemParamsFactory pathItemParamsFactory,
            ISubPathItemComparer comparer,
            IValidityPeriodService validityPeriodService,
            ILogger logger,
            IPeriodEndRepository periodEndRepository)
            : base(stateService, comparer.ConvertToBasePathItem(pathItems), pathItemParamsFactory, validityPeriodService, logger, periodEndRepository)
        {
        }

        public string Name => "Internal Reports Path";

        public override int PathId => Convert.ToInt32(PeriodEndPath.InternalReportsPath);

        public override string CollectionType => PeriodEndConstants.IlrCollectionNamePrefix;
    }
}