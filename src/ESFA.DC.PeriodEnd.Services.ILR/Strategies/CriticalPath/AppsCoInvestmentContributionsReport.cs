﻿using System.Collections.Generic;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath
{
    public class AppsCoInvestmentContributionsReport : BaseMultiProviderPeriodEndReport, IProviderReport
    {
        public AppsCoInvestmentContributionsReport(
            IQueryService queryService,
            IFileUploadJobManager jobManager,
            ILogger logger,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
            : base(queryService, jobManager, logger, jobFactory, returnFactory, stateService)
        {
        }

        public override List<int> ItemSubPaths => null;

        public override string DisplayName => "Apps Co-Investment Contributions Report";

        public override int PathItemId => PeriodEndPathItem.AppsCoInvestmentContributionsReport;

        protected override string CollectionName => PeriodEndConstants.CollectionName_AppsCoInvestmentContributionsReport;
    }
}