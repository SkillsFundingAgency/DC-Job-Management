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

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.InternalReportsPath
{
    public class EsfaNonContractedDevolvedAdultEducationActivityReport : AbstractBlockingPathItem, IInternalReportsPathItem
    {
        public EsfaNonContractedDevolvedAdultEducationActivityReport(
            ILogger logger,
            IFileUploadJobManager jobManager,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
            : base(logger, jobManager, jobFactory, returnFactory, stateService)
        {
        }

        public override string DisplayName => $"Period End ESFA Non-Contracted Devolved Adult Education Activity Report";

        public override string ReportFileName => "ESFA Non-Contracted Devolved Adult Education Activity Report";

        public override List<int> ItemSubPaths => null;

        public override int PathItemId => PeriodEndPathItem.ESFADevolvedAdultEducationReport;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.InternalReportsPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_EsfaNonContractedDevolvedAdultEducationActivityReport;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }
    }
}
