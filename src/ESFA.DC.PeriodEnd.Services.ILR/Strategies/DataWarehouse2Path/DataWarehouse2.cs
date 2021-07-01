using System;
using System.Collections.Generic;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Services.Strategies.ILR;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.DataWarehouse2Path
{
    public class DataWarehouse2 : AbstractDataSciencePathItem, IDataWarehouse2PathItem
    {
        public DataWarehouse2(
            ILogger logger,
            IFileUploadJobManager jobManager,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IValidityPeriodService validityPeriodService)
        : base(logger, jobManager, jobFactory, returnFactory, stateService, validityPeriodService)
        {
        }

        public List<int> ItemSubPaths => null;

        public override string DisplayName => "Data Warehouse 2";

        public override int PathItemId => PeriodEndPathItem.DataWarehouse2;

        public string CollectionNameInDatabase => CollectionName;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.DataWarehouse2Path);

        protected override string CollectionName => PeriodEndConstants.CollectionName_DataWarehouse2;
    }
}