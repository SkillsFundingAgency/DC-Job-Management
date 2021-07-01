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
    public class HiddenDataWarehouse2 : AbstractHiddenPathItem
    {
        private readonly IDataWarehouse2Controller _dataWarehouse2Controller;

        public HiddenDataWarehouse2(
            ILogger logger,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IDataWarehouse2Controller dataWarehouse2Controller,
            IPeriodEndRepository periodEndRepository)
            : base(logger, returnFactory, stateService, periodEndRepository)
        {
            _dataWarehouse2Controller = dataWarehouse2Controller;
        }

        public override string DisplayName => "Start Data Warehouse 2 SubPath";

        public override List<int> ItemSubPaths => new List<int> { _dataWarehouse2Controller.PathId };

        public override int PathItemId => PeriodEndPathItem.HiddenDataWarehouse2;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_DasPeriodEndReports;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }
    }
}