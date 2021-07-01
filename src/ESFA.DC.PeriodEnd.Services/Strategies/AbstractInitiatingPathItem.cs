using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Services.Strategies
{
    public abstract class AbstractInitiatingPathItem : AbstractBlockingPathItem
    {
        private readonly ILogger _logger;
        private readonly IPathItemReturnFactory _returnFactory;
        private readonly IStateService _stateService;

        protected AbstractInitiatingPathItem(
            ILogger logger,
            IFileUploadJobManager fileUploadJobManager,
            IPeriodEndJobFactory periodEndJobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
        : base(logger, fileUploadJobManager, periodEndJobFactory, returnFactory, stateService)
        {
            _logger = logger;
            _returnFactory = returnFactory;
            _stateService = stateService;
        }

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            try
            {
                await _stateService.SavePathItem(
                    new PathItemModel
                    {
                        PathId = PathId,
                        Ordinal = pathItemParams.Ordinal,
                        Name = DisplayName,
                        IsPausing = IsPausing,
                        HasJobs = false,
                        PathItemJobs = new List<PathItemJobModel>()
                    },
                    pathItemParams.CollectionYear,
                    pathItemParams.Period);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

            return _returnFactory.CreatePathTaskReturn(IsBlockingTask, Enumerable.Empty<long>(), ItemSubPaths);
        }

        public override async Task<bool> IsValidForPeriod(int period, int collectionYear, IDictionary<string, IEnumerable<int>> validities)
        {
            return true;
        }
    }
}