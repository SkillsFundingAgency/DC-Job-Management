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
    public class HiddenDataWarehouse1 : AbstractHiddenPathItem
    {
        private readonly IDataWarehouse1Controller _dataWarehouse1Controller;

        public HiddenDataWarehouse1(
            ILogger logger,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IDataWarehouse1Controller dataWarehouse1Controller,
            IPeriodEndRepository periodEndRepository)
            : base(logger, returnFactory, stateService, periodEndRepository)
        {
            _dataWarehouse1Controller = dataWarehouse1Controller;
        }

        public override string DisplayName => "Start Data Warehouse 1 SubPath";

        public override List<int> ItemSubPaths => new List<int> { _dataWarehouse1Controller.PathId };

        public override int PathItemId => PeriodEndPathItem.HiddenDataWarehouse1;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_DasStart;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }
    }
}