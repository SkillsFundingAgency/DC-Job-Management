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
    public class HiddenStandardFile : AbstractHiddenPathItem
    {
        private readonly IStandardFileController _standardFileController;

        public HiddenStandardFile(
            ILogger logger,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IStandardFileController standardFileController,
            IPeriodEndRepository periodEndRepository)
            : base(logger, returnFactory, stateService, periodEndRepository)
        {
            _standardFileController = standardFileController;
        }

        public override string DisplayName => "Start Standard File SubPath";

        public override List<int> ItemSubPaths => new List<int> { _standardFileController.PathId };

        public override int PathItemId => PeriodEndPathItem.HiddenStandardFile;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_AppsInternalDataMatchMonthEndReport;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }
    }
}