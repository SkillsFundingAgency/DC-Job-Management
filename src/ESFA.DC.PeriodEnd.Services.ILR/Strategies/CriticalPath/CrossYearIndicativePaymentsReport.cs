using System;
using System.Collections.Generic;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath
{
    public class CrossYearIndicativePaymentsReport : BaseMultiProviderPeriodEndReport
    {
        public CrossYearIndicativePaymentsReport(
            IQueryService queryService,
            IFileUploadJobManager jobManager,
            ILogger logger,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
            : base(queryService, jobManager, logger, jobFactory, returnFactory, stateService)
        {
        }

        public override string DisplayName => "Beta Cross Year Indicative Payments Report";

        public override List<int> ItemSubPaths => null;

        public override int PathItemId => PeriodEndPathItem.CrossYearIndicativePaymentsReport;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_CrossYearIndicativePaymentsReport;
    }
}