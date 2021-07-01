using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItemControllers;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath
{
    public class DASRunning : AbstractBlockingPathItem
    {
        private readonly IDASStartedController _dasStartedController;
        private readonly IPeriodEndRepository _periodEndRepository;

        public DASRunning(
            ILogger logger,
            IFileUploadJobManager jobManager,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IDASStartedController dasStartedController,
            IPeriodEndRepository periodEndRepository)
            : base(logger, jobManager, jobFactory, returnFactory, stateService)
        {
            _dasStartedController = dasStartedController;
            _periodEndRepository = periodEndRepository;
        }

        public override string DisplayName => "DAS Period End Processing";

        public override List<int> ItemSubPaths => new List<int> { _dasStartedController.PathId };

        public override int PathItemId => PeriodEndPathItem.DASRun;

        public override bool IsHidden => false;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_DasRun;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            return await ExecuteAsync(
                pathItemParams.Ordinal,
                pathItemParams.CollectionYear,
                pathItemParams.Period);
        }

        public override async Task<bool> IsValidForPeriod(int period, int collectionYear, IDictionary<string, IEnumerable<int>> validities)
        {
            if (await base.IsValidForPeriod(period, collectionYear, validities))
            {
                return true;
            }

            var subpathValid = await _periodEndRepository.GetValidityForSubPathAsync(ItemSubPaths.First(), collectionYear, period, CancellationToken.None);

            return subpathValid;
        }
    }
}