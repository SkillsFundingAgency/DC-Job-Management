using System;
using System.Collections.Generic;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.FCSHandOverPart1Path
{
    public class FCSPart1InitialBlockingItem : AbstractInitiatingPathItem, IFCSHandOverPart1PathItem
    {
        public FCSPart1InitialBlockingItem(
            ILogger logger,
            IFileUploadJobManager jobManager,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
            : base(logger, jobManager, jobFactory, returnFactory, stateService)
        {
        }

        public override string DisplayName => "Initiate FCS hand over part 1";

        public override bool IsInitiating => true;

        public override List<int> ItemSubPaths => null;

        public override int PathItemId => PeriodEndPathItem.InitiateFCSPart1Handover;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.FCSHandOverPath1);

        protected override string CollectionName => string.Empty;
    }
}