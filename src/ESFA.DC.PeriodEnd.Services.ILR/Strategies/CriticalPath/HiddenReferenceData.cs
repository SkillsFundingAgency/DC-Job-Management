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
    public class HiddenReferenceData : AbstractHiddenPathItem
    {
        private readonly IReferenceDataController _referenceDataController;

        public HiddenReferenceData(
            ILogger logger,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IReferenceDataController referenceDataController,
            IPeriodEndRepository periodEndRepository)
            : base(logger, returnFactory, stateService, periodEndRepository)
        {
            _referenceDataController = referenceDataController;
        }

        public override string DisplayName => "Start Reference Data SubPath";

        public override List<int> ItemSubPaths => new List<int> { _referenceDataController.PathId };

        public override int PathItemId => PeriodEndPathItem.HiddenReferenceData;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_ReferenceDataEPA;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }
    }
}