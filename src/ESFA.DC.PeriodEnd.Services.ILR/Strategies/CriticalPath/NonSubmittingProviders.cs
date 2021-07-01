using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Services.Strategies;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath
{
    public class NonSubmittingProviders : AbstractPathItemBase
    {
        private readonly IQueryService _organisationService;
        private readonly IFileUploadJobManager _jobManager;
        private readonly ILogger _logger;
        private readonly IPeriodEndJobFactory _jobFactory;
        private readonly IPathItemReturnFactory _returnFactory;
        private readonly IStateService _stateService;

        public NonSubmittingProviders(
            IQueryService organisationService,
            IFileUploadJobManager jobManager,
            ILogger logger,
            IPeriodEndJobFactory jobFactory,
            IPathItemReturnFactory returnFactory,
            IStateService stateService)
        {
            _organisationService = organisationService;
            _jobManager = jobManager;
            _logger = logger;
            _jobFactory = jobFactory;
            _returnFactory = returnFactory;
            _stateService = stateService;
        }

        public override string DisplayName => "DAS Period End Submissions";

        public override int PathItemId => PeriodEndPathItem.NonSubmittingProviders;

        public override bool IsPausing => true;

        protected int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_DasNonSubmitting;

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

            var nonSubmittingProviders = (await _organisationService
                .GetNonSubmittingProviders(pathItemParams.CollectionName, pathItemParams.Period))
                .ToList();

            _logger.LogDebug($"Found {nonSubmittingProviders.Count} latest submitted providers.");

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
                foreach (var provider in nonSubmittingProviders)
                {
                    var jobId = await _jobManager.AddJob(
                        await _jobFactory.CreateJobAsync(
                            GetYearSpecificCollectionName(pathItemParams.CollectionYear),
                            pathItemParams.CollectionYear,
                            pathItemParams.Period,
                            provider.UkPrn,
                            provider.StorageReference,
                            provider.FileName));

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

            pathItem.HasJobs = pathItemJobs.Any();

            pathItem.PathItemJobs = pathItemJobs.Any() ? pathItemJobs : null;

            await _stateService.SavePathItem(pathItem, pathItemParams.CollectionYear, pathItemParams.Period);

            return _returnFactory.CreatePathTaskReturn(true, jobIds.Any() ? jobIds : null, null);
        }

        private string GetYearSpecificCollectionName(int collectionYear)
        {
            return CollectionName.Replace(PeriodEndConstants.CollectionNameYearToken, collectionYear.ToString());
        }
    }
}