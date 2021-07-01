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
    public class HiddenPeriodEndStart : AbstractHiddenPathItem
    {
        private readonly IInternalReportsController _internalReportsController;

        public HiddenPeriodEndStart(
            ILogger logger,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IInternalReportsController internalReportsController,
            IPeriodEndRepository periodEndRepository)
            : base(logger, returnFactory, stateService, periodEndRepository)
        {
            _internalReportsController = internalReportsController;
        }

        public override string DisplayName => "Start Internal Reports SubPath";

        public override List<int> ItemSubPaths => new List<int> { _internalReportsController.PathId };

        public override int PathItemId => PeriodEndPathItem.HiddenPeriodEndStarted;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_DataQualityReport;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }
    }
}