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
    public class FCSHandOverPart1Controller : AbstractSubPathController, IFCSHandOverPart1Controller
    {
        public FCSHandOverPart1Controller(
            IStateService stateService,
            IOrderedEnumerable<IFCSHandOverPart1PathItem> pathItems,
            IPathItemParamsFactory pathItemParamsFactory,
            ISubPathItemComparer comparer,
            IValidityPeriodService validityPeriodService,
            ILogger logger,
            IPeriodEndRepository periodEndRepository)
            : base(stateService, comparer.ConvertToBasePathItem(pathItems), pathItemParamsFactory, validityPeriodService, logger, periodEndRepository)
        {
        }

        public string Name => "FCS Hand Over Part 1 Path";

        public override int PathId => Convert.ToInt32(PeriodEndPath.FCSHandOverPath1);

        public override string CollectionType => PeriodEndConstants.IlrCollectionNamePrefix;
    }
}