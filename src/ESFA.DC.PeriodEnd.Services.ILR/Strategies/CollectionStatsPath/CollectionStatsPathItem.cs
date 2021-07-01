using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.CollectionStatsPath
{
    public class CollectionStatsPathItem : AbstractBlockingPathItem, IInternalReportsPathItem
    {
        public CollectionStatsPathItem(
            ILogger logger,
            IFileUploadJobManager jobManager,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
            : base(logger, jobManager, jobFactory, returnFactory, stateService)
        {
        }

        public override string DisplayName => "Period End Collection Stats";

        public override string ReportFileName => "CollectionStats.json";

        public override List<int> ItemSubPaths => null;

        public override int PathItemId => PeriodEndPathItem.CollectionStats;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.InternalReportsPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_CollectionStats;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }
    }
}