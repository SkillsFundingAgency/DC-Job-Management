using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Services.Strategies
{
    public abstract class AbstractBlockingPathItem : AbstractYearSpecificPathItem
    {
        private readonly IFileUploadJobManager _jobManager;
        private readonly ILogger _logger;
        private readonly IPeriodEndJobFactory _jobFactory;
        private readonly IPathItemReturnFactory _returnFactory;
        private readonly IStateService _stateService;

        protected AbstractBlockingPathItem(
            ILogger logger,
            IFileUploadJobManager jobManager,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
        {
            _jobManager = jobManager;
            _logger = logger;
            _jobFactory = jobFactory;
            _returnFactory = returnFactory;
            _stateService = stateService;
        }

        public override bool IsPausing => true;

        protected bool IsBlockingTask => true;

        protected abstract int PathId { get; }

        public async override Task<bool> IsValidForPeriod(
            int period,
            int collectionYear,
            IDictionary<string, IEnumerable<int>> validities)
        {
            if (!validities.TryGetValue(GetValidityKey(), out var validPeriods))
            {
                return false;
            }

            return validPeriods.Contains(period);
        }

        protected async Task<PathItemReturn> ExecuteAsync(int ordinal, int collectionYear, int period)
        {
            _logger.LogInfo($"In {GetType().Name}");

            var jobId = await _jobManager.AddJob(await _jobFactory.CreateJobAsync(GetYearSpecificCollectionName(collectionYear), collectionYear, period));

            _logger.LogDebug($"Created Job {jobId}");

            try
            {
                await _stateService.SavePathItem(
                    new PathItemModel
                    {
                        PathId = PathId,
                        Ordinal = ordinal,
                        IsPausing = IsPausing,
                        Name = DisplayName,
                        HasJobs = true,
                        PathItemJobs = new List<PathItemJobModel>
                        {
                            new PathItemJobModel
                            {
                                JobId = jobId,
                                Status = Convert.ToInt32(JobStatusType.Ready)
                            }
                        }
                    },
                    collectionYear,
                    period);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

            return _returnFactory.CreatePathTaskReturn(IsBlockingTask, new List<long> { jobId }, ItemSubPaths);
        }
    }
}