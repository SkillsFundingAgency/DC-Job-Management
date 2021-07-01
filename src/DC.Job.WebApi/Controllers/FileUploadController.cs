using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Job.WebApi.Providers;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/file-upload")]
    public class FileUploadController : ControllerBase
    {
        private readonly IStorageServiceProvider _storageServiceProvider;
        private readonly ILogger _logger;
        private readonly IFileUploadJobManager _fileUploadMetaDataManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJobQueryService _jobQueryService;
        private readonly ICollectionService _collectionService;
        private readonly string _reportFileName = "{0}/{1}/Rule Violation Report {2}.json";

        public FileUploadController(
            IStorageServiceProvider storageServiceProvider,
            ILogger logger,
            IFileUploadJobManager fileUploadMetaDataManager,
            IDateTimeProvider dateTimeProvider,
            IJobQueryService jobQueryService,
            ICollectionService collectionService)
        {
            _storageServiceProvider = storageServiceProvider;
            _logger = logger;
            _fileUploadMetaDataManager = fileUploadMetaDataManager;
            _dateTimeProvider = dateTimeProvider;
            _jobQueryService = jobQueryService;
            _collectionService = collectionService;
        }

        [HttpPost("upload/{jobId}")]
        public async Task<IActionResult> Post(CancellationToken cancellationToken, long jobId)
        {
            _logger.LogInfo($"Request recieved to create a new job based on Job id : {jobId}");
            var existingJob = await _jobQueryService.GetJobById(jobId);
            if (existingJob == null)
            {
                _logger.LogInfo($"Bad Request recieved to create a new job based on Job id : {jobId}");
                return BadRequest("invalid job id");
            }

            if (Request?.Form?.Files == null || !Request.Form.Files.Any())
            {
                _logger.LogInfo($"Bad Request recieved to create a new job based on Job id : {jobId} - no files attached");
                return BadRequest("invalid job id");
            }

            var uploadedFile = Request.Form.Files.FirstOrDefault();

            _logger.LogInfo($"Going to create a new job based on Job id : {jobId} with filename : {uploadedFile.FileName}");

            var collection = await _collectionService.GetCollectionFromNameAsync(cancellationToken, existingJob.CollectionName);

            var newjob = new FileUploadJob
            {
                CollectionName = existingJob.CollectionName,
                CrossLoadingStatus = 0,
                DateTimeSubmittedUtc = _dateTimeProvider.GetNowUtc(),
                IsFirstStage = false,
                NotifyEmail = existingJob.NotifyEmail,
                CreatedBy = $"{existingJob.CreatedBy}*",
                PeriodNumber = existingJob.PeriodNumber,
                Priority = 1,
                Status = JobStatusType.Ready,
                Ukprn = existingJob.Ukprn,
                StorageReference = collection.StorageReference,
                TermsAccepted = true,
            };

            newjob.FileName = $"{existingJob.Ukprn}/{uploadedFile.FileName}".ToUpper();
            newjob.FileSize = uploadedFile.Length / 1024;

            var newjobId = await _fileUploadMetaDataManager.AddJob(newjob);

            // TODO: First Stage set to true by default - need to change the actual AddJob call
            await _fileUploadMetaDataManager.SubmitIlrJob(newjobId);

            _logger.LogInfo($"Created new job based on Job id : {jobId} new job Id : {newjobId}");

            await (await _storageServiceProvider.GetAzureStorageReferenceServiceAsync(cancellationToken, existingJob.CollectionName))
                .SaveAsync(newjob.FileName, uploadedFile.OpenReadStream(), cancellationToken);

            _logger.LogInfo($"Uploaded new file based on new job id : {newjobId} file name : {uploadedFile.FileName}");

            return new OkObjectResult(newjobId);
        }
    }
}