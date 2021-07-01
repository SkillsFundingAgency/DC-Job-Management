using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.NCS.Strategies
{
    public class NCSSummarisation : AbstractBlockingPathItem
    {
        public NCSSummarisation(
            ILogger logger,
            IFileUploadJobManager jobManager,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
            : base(logger, jobManager, jobFactory, returnFactory, stateService)
        {
        }

        public override string DisplayName => "Period End NCS Summarisation";

        public override List<int> ItemSubPaths => null;

        public override int PathItemId => PeriodEndPathItem.NCSSummarisation;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.NCSCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_NCSSummarisation;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }
    }
}