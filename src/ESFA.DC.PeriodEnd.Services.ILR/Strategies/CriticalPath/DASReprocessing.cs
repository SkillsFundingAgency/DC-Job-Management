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
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.PeriodEnd.Services.ILR.Strategies.CriticalPath
{
    public class DASReprocessing : AbstractBlockingPathItem
    {
        private readonly ILogger _logger;
        private readonly IFileUploadJobManager _jobManager;
        private readonly IPeriodEndJobFactory _jobFactory;
        private readonly IJsonSerializationService _serializationService;
        private readonly IPathItemReturnFactory _returnFactory;
        private readonly IStateService _stateService;
        private readonly IQueryService _queryService;
        private readonly IDASClientService _dasClientService;

        public DASReprocessing(
            ILogger logger,
            IFileUploadJobManager jobManager,
            IPeriodEndJobFactory jobFactory,
            IJsonSerializationService serializationService,
            IPathItemReturnFactory returnFactory,
            IStateService stateService,
            IQueryService queryService,
            IDASClientService dasClientService)
            : base(logger, jobManager, jobFactory, returnFactory, stateService)
        {
            _logger = logger;
            _jobManager = jobManager;
            _jobFactory = jobFactory;
            _serializationService = serializationService;
            _returnFactory = returnFactory;
            _stateService = stateService;
            _queryService = queryService;
            _dasClientService = dasClientService;
        }

        public override string DisplayName => "DAS Period End Reprocessing";

        public override List<int> ItemSubPaths => null;

        public override int PathItemId => PeriodEndPathItem.DASReprocess;

        protected override int PathId => Convert.ToInt32(PeriodEndPath.ILRCriticalPath);

        protected override string CollectionName => PeriodEndConstants.CollectionName_DasNonSubmitting;

        public override async Task<PathItemReturn> ExecuteAsync(PathItemParams pathItemParams)
        {
            _logger.LogInfo($"In {GetType().Name}");

            var jobIds = new List<long>();

            var pathItem = new PathItemModel
            {
                Name = DisplayName,
                PathId = PathId,
                Ordinal = pathItemParams.Ordinal,
                IsPausing = IsPausing
            };

            var pathItemJobs = new List<PathItemJobModel>();

            try
            {
                var ilrCollectionName = GetYearSpecificCollectionName(pathItemParams.CollectionYear, PeriodEndConstants.CollectionName_ILRSubmission);

                var expectedDasSubmissions = (await _queryService.GetLatestDASSubmittedJobs(ilrCollectionName, GetYearSpecificCollectionName(pathItemParams.CollectionYear))).ToList();
                _logger.LogInfo("Expected Return " + _serializationService.Serialize(expectedDasSubmissions));

                // call DAS API
                var dasSubmissions = (await _dasClientService.GetDASProcessedProviders(pathItemParams.CollectionYear, pathItemParams.Period)).ToDictionary(r => r.JobId);
                _logger.LogInfo("DAS Submission Return " + _serializationService.Serialize(dasSubmissions));

                var nonProcessedProviders = expectedDasSubmissions.Where(p => !dasSubmissions.ContainsKey(p.JobId)).ToList();
                _logger.LogInfo("Not Processed " + _serializationService.Serialize(nonProcessedProviders));

                foreach (var provider in nonProcessedProviders)
                {
                    var job = await _jobFactory.CreateJobAsync(
                        GetYearSpecificCollectionName(pathItemParams.CollectionYear, PeriodEndConstants.CollectionName_DasNonSubmitting),
                        pathItemParams.CollectionYear,
                        pathItemParams.Period,
                        provider.UkPrn);

                    var jobId = await _jobManager.AddJob(job);

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
    }
}