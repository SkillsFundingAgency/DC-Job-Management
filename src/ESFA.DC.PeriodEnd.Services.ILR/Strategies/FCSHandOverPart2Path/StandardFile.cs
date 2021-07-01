using System;
using System.Collections.Generic;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Services.Strategies.ILR;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.FCSHandOverPart2Path
{
    public class StandardFile : AbstractDataSciencePathItem, IFCSHandOverPart2PathItem
    {
        public StandardFile(
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

        public override string DisplayName => "Standard File";

        public override int PathItemId => PeriodEndPathItem.StandardFile;

        public string CollectionNameInDatabase => CollectionName;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.FCSHandOverPath2);

        protected override string CollectionName => PeriodEndConstants.CollectionName_StandardFile;
    }
}