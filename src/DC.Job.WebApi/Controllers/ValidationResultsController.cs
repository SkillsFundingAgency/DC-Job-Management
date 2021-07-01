using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.Services.Interfaces;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Job.WebApi.Providers;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class ValidationResultsController : ControllerBase
    {
        private readonly IStorageServiceProvider _storageServiceProvider;
        private readonly ILogger _logger;
        private readonly IJobQueryService _jobQueryService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IIlrDataStoreService _ilrDataStoreService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly string _reportFileName = "{0}/{1}/Rule Violation Report {2}.json";

        public ValidationResultsController(
            IStorageServiceProvider storageServiceProvider,
            ILogger logger,
            IJobQueryService jobQueryService,
            IDateTimeProvider dateTimeProvider,
            IIlrDataStoreService ilrDataStoreService,
            IJsonSerializationService jsonSerializationService)
        {
            _storageServiceProvider = storageServiceProvider;
            _logger = logger;
            _jobQueryService = jobQueryService;
            _dateTimeProvider = dateTimeProvider;
            _ilrDataStoreService = ilrDataStoreService;
            _jsonSerializationService = jsonSerializationService;
        }

        [HttpGet("{ukprn}/{jobId}")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken, long ukprn, long jobId)
        {
            _logger.LogInfo($"Get request recieved for validation errors for ukprn {ukprn}, job id: {jobId}", jobIdOverride: jobId);

            if (jobId < 1 || ukprn < 1)
            {
                _logger.LogWarning($"invalid jobId : {jobId}, ukprn : {ukprn}");
                return new BadRequestResult();
            }

            var job = await _jobQueryService.GetJobById(jobId);
            if (job == null || job.Ukprn != ukprn)
            {
                _logger.LogWarning($"No job found for jobId : {jobId}, ukprn : {ukprn}", jobIdOverride: jobId);
                return new BadRequestResult();
            }

            var fileName = GetFileName(ukprn, jobId, job.DateTimeSubmittedUtc);
            try
            {
                var keyValuePersistenceService = await GetKeyValuePersistenceServiceAsync(cancellationToken, job.CollectionName);
                var exists = await keyValuePersistenceService.ContainsAsync(fileName);
                if (exists)
                {
                    var data = await keyValuePersistenceService.GetAsync(fileName);
                    var validationResults = _jsonSerializationService.Deserialize<FileValidationResult>(data);

                    var previousSubmission = await _ilrDataStoreService.GetLatestIlrSubmissionDetails(job.CollectionYear, job.Ukprn);
                    validationResults.PreviouslySubmittedLearnerCount = previousSubmission?.TotalLearnersSubmitted;

                    return Ok(validationResults);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured trying to get validation errors for file name : {fileName}", ex);
                return new BadRequestResult();
            }

            return new NotFoundResult();
        }

        public string GetFileName(long ukprn, long jobId, DateTime dateTimeUtc)
        {
            var jobDateTime = _dateTimeProvider.ConvertUtcToUk(dateTimeUtc).ToString("yyyyMMdd-HHmmss");
            return string.Format(_reportFileName, ukprn, jobId, jobDateTime);
        }

        public async Task<IKeyValuePersistenceService> GetKeyValuePersistenceServiceAsync(CancellationToken cancellationToken, string collectionName)
        {
            return await _storageServiceProvider.GetAzureStorageReferenceServiceAsync(cancellationToken, collectionName);
        }
    }
}