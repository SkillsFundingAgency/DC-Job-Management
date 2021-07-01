using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath
{
    public class HiddenFCSPart2HandOver : AbstractHiddenPathItem
    {
        private readonly IFCSHandOverPart2Controller _fcsHandOverController;

        public HiddenFCSPart2HandOver(
            ILogger logger,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IFCSHandOverPart2Controller fcsHandOverController,
            IPeriodEndRepository periodEndRepository)
            : base(logger, returnFactory, stateService, periodEndRepository)
        {
            _fcsHandOverController = fcsHandOverController;
        }

        public override string DisplayName => "Start FCS Handover Part 2 SubPath";

        public override List<int> ItemSubPaths => new List<int> { _fcsHandOverController.PathId };

        public override int PathItemId => PeriodEndPathItem.HiddenFCSPart2Handover;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_AppSummarisation;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }
    }
}