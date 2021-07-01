using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath
{
    public class DataExtractReport : AbstractBlockingPathItem
    {
        public DataExtractReport(
            ILogger logger,
            IFileUploadJobManager jobManager,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
            : base(logger, jobManager, jobFactory, returnFactory, stateService)
        {
        }

        public override string DisplayName => "Period End Data Extract Report";

        public override string ReportFileName => "Data Extract Report R";

        public override List<int> ItemSubPaths => null;

        public override int PathItemId => PeriodEndPathItem.DataExtractReport;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_DataExtractReport;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }
    }
}