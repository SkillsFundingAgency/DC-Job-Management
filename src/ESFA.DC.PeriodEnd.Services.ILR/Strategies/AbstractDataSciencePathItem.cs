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

namespace ESFA.DC.PeriodEnd.Services.Strategies.ILR
{
    public abstract class AbstractDataSciencePathItem : AbstractYearSpecificPathItem
    {
        private readonly ILogger _logger;
        private readonly IFileUploadJobManager _jobManager;
        private readonly IPeriodEndJobFactory _jobFactory;
        private readonly IPathItemReturnFactory _returnFactory;
        private readonly IStateService _stateService;
        private readonly IValidityPeriodService _validityPeriodService;

        protected AbstractDataSciencePathItem(
            ILogger logger,
            IFileUploadJobManager jobManager,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IValidityPeriodService validityPeriodService)
        {
            _logger = logger;
            _jobManager = jobManager;
            _jobFactory = jobFactory;
            _returnFactory = returnFactory;
            _stateService = stateService;
            _validityPeriodService = validityPeriodService;
        }

        protected virtual int PathId => 0;

        private bool IsBlocking => false;

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

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            _logger.LogInfo($"In {GetType().Name}");

            var jobIds = new List<long>();

            var pathItem = new PathItemModel
            {
                PathId = PathId,
                Ordinal = pathItemParams.Ordinal,
                Name = DisplayName,
                IsPausing = IsPausing
            };

            var pathItemJobs = new List<PathItemJobModel>();

            try
            {
                var jobId = await _jobManager.AddJob(
                    await _jobFactory.CreateJobAsync(
                        GetYearSpecificCollectionName(pathItemParams.CollectionYear),
                        pathItemParams.CollectionYear,
                        pathItemParams.Period));

                _logger.LogDebug($"Created Job {jobId}");
                jobIds.Add(jobId);

                pathItemJobs.Add(new PathItemJobModel
                {
                    JobId = jobId,
                    Status = Convert.ToInt32(JobStatusType.Ready)
                });

                pathItem.HasJobs = true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

            pathItem.PathItemJobs = pathItemJobs;

            await _stateService.SavePathItem(pathItem, pathItemParams.CollectionYear, pathItemParams.Period);

            return _returnFactory.CreatePathTaskReturn(IsBlocking, jobIds, Enumerable.Empty<int>());
        }
    }
}