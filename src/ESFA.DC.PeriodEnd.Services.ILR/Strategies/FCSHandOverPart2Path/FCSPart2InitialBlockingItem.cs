using System;
using System.Collections.Generic;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.FCSHandOverPart2Path
{
    public class FCSPart2InitialBlockingItem : AbstractInitiatingPathItem, IFCSHandOverPart2PathItem
    {
        public FCSPart2InitialBlockingItem(
            ILogger logger,
            IFileUploadJobManager jobManager,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
            : base(logger, jobManager, jobFactory, returnFactory, stateService)
        {
        }

        public override string DisplayName => "Initiate FCS hand over part 2";

        public override List<int> ItemSubPaths => null;

        public override bool IsInitiating => true;

        public override int PathItemId => PeriodEndPathItem.InitiateFCSPart2Handover;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.FCSHandOverPath2);

        protected override string CollectionName => string.Empty;
    }
}