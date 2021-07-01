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
    public class HiddenDataExtract : AbstractHiddenPathItem
    {
        private readonly IDataExtractController _dataExtractController;

        public HiddenDataExtract(
            ILogger logger,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IDataExtractController dataExtractController,
            IPeriodEndRepository periodEndRepository)
            : base(logger, returnFactory, stateService, periodEndRepository)
        {
            _dataExtractController = dataExtractController;
        }

        public override string DisplayName => "Start Data Extract SubPath";

        public override List<int> ItemSubPaths => new List<int> { _dataExtractController.PathId };

        public override int PathItemId => PeriodEndPathItem.HiddenDataExtract;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_DataExtractReport;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }
    }
}