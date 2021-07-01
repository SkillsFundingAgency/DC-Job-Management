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
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath
{
    public class McaReport : AbstractPathItemBase
    {
        private readonly IMcaDetailService _mcaDetailService;
        private readonly IFileUploadJobManager _jobManager;
        private readonly ILogger _logger;
        private readonly IPeriodEndJobFactory _jobFactory;
        private readonly IPathItemReturnFactory _returnFactory;
        private readonly IStateService _stateService;
        private readonly IReturnPeriodService _returnPeriodService;
        private readonly IValidityPeriodService _validityPeriodService;

        public McaReport(
            IMcaDetailService mcaDetailService,
            IFileUploadJobManager jobManager,
            ILogger logger,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IReturnPeriodService returnPeriodService,
            IValidityPeriodService validityPeriodService)
        {
            _mcaDetailService = mcaDetailService;
            _jobManager = jobManager;
            _logger = logger;
            _jobFactory = jobFactory;
            _returnFactory = returnFactory;
            _stateService = stateService;
            _returnPeriodService = returnPeriodService;
            _validityPeriodService = validityPeriodService;
        }

        public override string DisplayName => "MCA Reports";

        public override int PathItemId => PeriodEndPathItem.MCAReports;

        public override bool IsPausing => true;

        protected int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_McaReport;

        public override async Task<bool> IsValidForPeriod(
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

            var periodEndDate =
                await _returnPeriodService.GetPeriodEndEndDate(pathItemParams.CollectionName, pathItemParams.Period);

            var mcaList = (await _mcaDetailService
                .GetActiveMcaListAsync(pathItemParams.CollectionYear, periodEndDate))
                .ToList();

            _logger.LogDebug($"Found {mcaList.Count} ukprns. [{string.Join(", ", mcaList.Select(u => u.ToString()))}]");

            var pathItem = new PathItemModel
            {
                PathId = PathId,
                Name = DisplayName,
                Ordinal = pathItemParams.Ordinal,
                IsPausing = IsPausing
            };

            var pathItemJobs = new List<PathItemJobModel>();

            try
            {
                foreach (var provider in mcaList)
                {
                    var jobId = await _jobManager.AddJob(
                        await _jobFactory.CreateJobAsync(
                            GetYearSpecificCollectionName(pathItemParams.CollectionYear),
                            pathItemParams.CollectionYear,
                            pathItemParams.Period,
                            provider));

                    _logger.LogDebug($"Created Job {jobId}");
                    jobIds.Add(jobId);

                    pathItemJobs.Add(new PathItemJobModel
                    {
                        JobId = jobId,
                        Status = Convert.ToInt32(JobStatusType.Ready)
                    });
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

            pathItem.PathItemJobs = pathItemJobs;

            pathItem.HasJobs = pathItemJobs.Any();

            await _stateService.SavePathItem(pathItem, pathItemParams.CollectionYear, pathItemParams.Period);

            return _returnFactory.CreatePathTaskReturn(pathItemJobs.Any(), jobIds.Any() ? jobIds : null, null);
        }

        private string GetYearSpecificCollectionName(int collectionYear)
        {
            return CollectionName.Replace(PeriodEndConstants.CollectionNameYearToken, collectionYear.ToString());
        }
    }
}