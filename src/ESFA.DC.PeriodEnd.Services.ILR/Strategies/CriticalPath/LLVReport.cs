using System;
using System.Collections.Generic;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath
{
    public class LLVReport : BaseMultiProviderPeriodEndReport
    {
        public LLVReport(
            IQueryService queryService,
            IFileUploadJobManager jobManager,
            ILogger logger,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
            : base(queryService, jobManager, logger, jobFactory, returnFactory, stateService)
        {
        }

        public override string DisplayName => "Learner Level View Report";

        public override List<int> ItemSubPaths => null;

        public override int PathItemId => PeriodEndPathItem.LLVReport;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_LLVReport;
    }
}