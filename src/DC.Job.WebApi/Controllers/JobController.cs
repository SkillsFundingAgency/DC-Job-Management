using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Api.Common.Utilities.Extensions;
using ESFA.DC.Audit.Models.DTOs.Job;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.JobQueueManager.Converters;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobScheduler.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/job")]
    public class JobController : BaseJobController<FileUploadJob>
    {
        private readonly IJobManager _jobManager;
        private readonly ILogger _logger;
        private readonly IFileUploadJobManager _fileUploadJobManager;
        private readonly IJobQueueDataContext _jobQueueDataContext;
        private readonly IReturnCalendarService _returnCalendarService;
        private readonly IJobQueryService _jobQueryService;
        private readonly IPeriodEndServiceILR _periodEndService;
        private readonly IFailedJobNotificationService _failedJobNotificationService;
        private readonly IStateService _periodEndStateService;
        private readonly IJobConverter _jobConverter;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJobManagementService _jobManagementService;
        private readonly IJobProcessingService _jobProcessingService;
        private readonly IAuditFactory _auditFactory;

        public JobController(
            IJobManager jobManager,
            ILogger logger,
            IFileUploadJobManager fileUploadMetaDataManager,
            IJobQueueDataContext jobQueueDataContext,
            IReturnCalendarService returnCalendarService,
            IJobQueryService jobQueryService,
            IJobConverter jobConverter,
            IPeriodEndServiceILR periodEndService,
            IFailedJobNotificationService failedJobNotificationService,
            IStateService periodEndStateService,
            IJobManagementService jobManagementService,
            Func<FileUploadJob> fileUploadFunc,
            IDateTimeProvider dateTimeProvider,
            IJobProcessingService jobProcessingService,
            IAuditFactory auditFactory)
            : base(fileUploadMetaDataManager, fileUploadFunc, logger)
        {
            _jobManager = jobManager;
            _logger = logger;
            _fileUploadJobManager = fileUploadMetaDataManager;
            _jobQueueDataContext = jobQueueDataContext;
            _returnCalendarService = returnCalendarService;
            _jobQueryService = jobQueryService;
            _jobConverter = jobConverter;
            _periodEndService = periodEndService;
            _failedJobNotificationService = failedJobNotificationService;
            _periodEndStateService = periodEndStateService;
            _dateTimeProvider = dateTimeProvider;
            _jobManagementService = jobManagementService;
            _jobProcessingService = jobProcessingService;
            _auditFactory = auditFactory;
        }

        #region Get Calls

        // GET: api/Job
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            _logger.LogInfo("Get request received for all jobs");

            try
            {
                var jobsList = (await _fileUploadJobManager.GetAllJobs()).ToList();

                jobsList = jobsList.OrderByDescending(x =>
                {
                    switch (x.Status)
                    {
                        case JobStatusType.Completed:
                            return 10;
                        case JobStatusType.Failed:
                            return 20;
                        case JobStatusType.FailedRetry:
                            return 30;
                        case JobStatusType.Paused:
                            return 40;
                        case JobStatusType.MovedForProcessing:
                        case JobStatusType.Processing:
                            return 50;
                        default:
                            return 60;
                    }
                }).ThenByDescending(x => x.Priority).ThenBy(x => x.JobId).ToList();

                _logger.LogInfo($"Returning jobs list with count: {jobsList.Count}");
                return Ok(jobsList.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError("Get all jobs failed", ex);
                return BadRequest();
            }
        }

        [HttpGet("{jobId}")]
        public async Task<IActionResult> GetByJobId(long jobId, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get the with Id: {jobId};", jobIdOverride: jobId);

            if (jobId == 0)
            {
                _logger.LogWarning($"Request received with Job id {jobId} 0", jobIdOverride: jobId);
                return BadRequest();
            }

            var job = await _jobQueryService.GetJobById(jobId, cancellationToken);

            _logger.LogInfo($"Returning job successfully with job id: {job.JobId}", jobIdOverride: jobId);
            return Ok(job);
        }

        [HttpGet("{ukprn}/{jobId}")]
        public async Task<IActionResult> GetById(long ukprn, long jobId, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get the with Id: {jobId}; ukprn: {ukprn}", jobIdOverride: jobId);

            if (jobId == 0)
            {
                _logger.LogWarning($"Request received with Job id {jobId} 0", jobIdOverride: jobId);
                return BadRequest();
            }

            var job = await _jobQueryService.GetJobById(jobId);
            if (job?.Ukprn != ukprn)
            {
                _logger.LogWarning($"Job id {jobId} with ukprn {ukprn} not found", jobIdOverride: jobId);
                return BadRequest();
            }

            _logger.LogInfo($"Returning job successfully with job id: {job.JobId}", jobIdOverride: jobId);
            return Ok(job);
        }

        [HttpGet("ukprn/{ukprn}")]
        public async Task<IActionResult> GetForUkprn(long ukprn, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get the with ukprn: {ukprn}");

            if (ukprn == 0)
            {
                _logger.LogWarning("Request received with ukprn 0");
                return BadRequest();
            }

            var jobsList = (await _jobQueryService.GetJobsByUkprnAsync(ukprn, cancellationToken)).OrderByDescending(x => x.DateTimeSubmittedUtc).ToList();

            _logger.LogInfo($"Returning {jobsList.Count} jobs successfully for ukprn: {ukprn}");
            return Ok(jobsList);
        }

        [HttpGet("ukprn/{ukprn}/collection/{collectionName}")]
        public async Task<IActionResult> GetForUkprnPerCollection(long ukprn, string collectionName, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get the with ukprn: {ukprn}");

            if (ukprn == 0 || string.IsNullOrEmpty(collectionName))
            {
                _logger.LogWarning($"Request received with ukprn {ukprn} and collectinName {collectionName}");
                return BadRequest();
            }

            var jobsList = (await _jobQueryService.GetJobsByUkprnPerCollectionAsync(ukprn, collectionName, cancellationToken)).OrderByDescending(x => x.DateTimeSubmittedUtc).ToList();

            _logger.LogInfo($"Returning {jobsList.Count} jobs successfully for ukprn: {ukprn} and collectionName {collectionName}");
            return Ok(jobsList);
        }

        [HttpGet("for-date/{startDateTimeUtc}/{endDateTimeUtc}")]
        public async Task<IActionResult> GetForDateRange(DateTime startDateTimeUtc, DateTime endDateTimeUtc, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to GetForDate start date :{startDateTimeUtc}, end date : {endDateTimeUtc}");

            var jobsList = (await _jobQueryService.GetJobsAsync(startDateTimeUtc: startDateTimeUtc, enDateTimeUtc: endDateTimeUtc)).OrderByDescending(x => x.DateTimeSubmittedUtc).ToList();

            _logger.LogInfo($"Returning {jobsList.Count} jobs successfully");
            return Ok(jobsList);
        }

        [HttpGet("{ukprn}/{startDateTimeUtc}/{endDateTimeUtc}/{collectionType?}/{status?}")]
        public async Task<IActionResult> GetForUkprn(long ukprn, DateTime startDateTimeUtc, DateTime endDateTimeUtc, string collectionType = null, short? status = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogInfo($"Request received to get the with ukprn: {ukprn}, start date :{startDateTimeUtc}, end date : {endDateTimeUtc}");

            if (ukprn == 0)
            {
                _logger.LogWarning("Request received with ukprn 0");
                return BadRequest();
            }

            var jobsList = (await _jobQueryService.GetJobsAsync(
                ukprn,
                startDateTimeUtc: startDateTimeUtc,
                enDateTimeUtc: endDateTimeUtc,
                isSubmitted: true,
                isCollectionUploadType: true,
                collectionType: collectionType,
                jobStatus: status)).ToList();

            _logger.LogInfo($"Returning {jobsList.Count} jobs successfully for ukprn: {ukprn}");
            return Ok(jobsList);
        }

        [HttpGet("{ukprn}/{startDateTimeUtc}/{endDateTimeUtc}/latest-for-period")]
        public async Task<IActionResult> GetLatestPerPeriodForUkprn(long ukprn, DateTime startDateTimeUtc, DateTime endDateTimeUtc, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to GetLatestPerPeriodForUkprn with ukprn: {ukprn}, start date :{startDateTimeUtc}, end date : {endDateTimeUtc}");

            if (ukprn == 0)
            {
                _logger.LogWarning("Request received with ukprn 0");
                return BadRequest();
            }

            var jobsList = (await _jobQueryService.GetLatestJobsPerPeriodByUkprnAsync(ukprn, startDateTimeUtc, endDateTimeUtc, cancellationToken)).OrderByDescending(x => x.DateTimeSubmittedUtc).ToList();

            _logger.LogInfo($"Returning {jobsList.Count} jobs successfully for ukprn: {ukprn}");
            return Ok(jobsList);
        }

        [HttpGet("{ukprn}/reports-history")]
        public async Task<IActionResult> GetJobsForReportsHistoryAsync(long ukprn, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to GetLatestPerPeriodForUkprn with ukprn: {ukprn}");

            if (ukprn == 0)
            {
                _logger.LogWarning("Request received with ukprn 0");
                return BadRequest();
            }

            var jobsList = (await _jobQueryService.GetLatestJobsPerPeriodForHistoryAsync(ukprn, cancellationToken)).ToList();

            _logger.LogInfo($"Returning {jobsList.Count} jobs successfully for ukprn: {ukprn}");
            return Ok(jobsList);
        }

        [HttpGet("ncs/{ukprn}/reports-history")]
        public async Task<IActionResult> GetNCSJobsForReportsHistoryAsync(long ukprn, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to GetLatestPerPeriodForUkprn with ukprn: {ukprn}");

            if (ukprn == 0)
            {
                _logger.LogWarning("Request received with ukprn 0");
                return BadRequest();
            }

            var jobsList = (await _jobQueryService.GetLatestNCSJobsPerPeriodForHistoryAsync(ukprn, cancellationToken)).ToList();

            _logger.LogInfo($"Returning {jobsList.Count} jobs successfully for ukprn: {ukprn}");
            return Ok(jobsList);
        }

        [HttpGet("{ukprn}/period/{period}")]
        public async Task<IActionResult> GetForPeriod(long ukprn, int period, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get the with ukprn: {ukprn}; period: {period}");

            if (ukprn == 0 || period == 0)
            {
                _logger.LogWarning($"Request received with ukprn {ukprn} and period {period} one of which is 0");
                return BadRequest();
            }

            var jobsList = (await _jobQueryService.GetJobsByUkprnForPeriodAsync(ukprn, period, cancellationToken)).ToList();

            _logger.LogInfo($"Returning {jobsList.Count} jobs successfully for ukprn :{ukprn}");
            return Ok(jobsList);
        }

        [HttpGet("period/{collectionName}/{failedOnly}/{ukprn?}")]
        public async Task<IActionResult> GetForCurrentPeriod(string collectionName, bool failedOnly, long? ukprn = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.LogInfo($"Request received to get for the current period for collection {collectionName}");

            if (string.IsNullOrEmpty(collectionName))
            {
                return BadRequest("Empty collection name");
            }

            var period = await _returnCalendarService.GetCurrentPeriodAsync(collectionName);
            if (period == null)
            {
                return BadRequest($"No open period for collection {collectionName}");
            }

            var jobsList = new List<FileUploadJob>();
            try
            {
                if (!failedOnly)
                {
                    var data = _jobQueueDataContext.FileUploadJobMetaData.Where(x => x.Job.IlrJobMetaData.Any() &&
                                                                                     x.PeriodNumber ==
                                                                                     period.PeriodNumber)
                        .Include(x => x.Job)
                        .Where(x => !ukprn.HasValue || x.Job.Ukprn == ukprn);

                    foreach (var jobMetaData in data)
                    {
                        var job = new FileUploadJob();
                        await _jobConverter.Convert(jobMetaData, job);
                        jobsList.Add(job);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured to get jobs", ex);
            }

            _logger.LogInfo($"Returning {jobsList.Count} jobs successfully for period :{period.PeriodNumber}, collection :{collectionName}");
            return Ok(jobsList);
        }

        [HttpGet("all-periods/{collectionName}/{statusCode?}")]
        public async Task<IActionResult> GetJobsForCollectionAsync(string collectionName, short? statusCode, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get for previous periods for collection {collectionName}");

            if (string.IsNullOrEmpty(collectionName))
            {
                return BadRequest("Empty collection name");
            }

            var jobs = (await _jobManagementService.GetJobsForAllPeriods(collectionName, statusCode, cancellationToken)).ToList();

            if (!jobs.Any())
            {
                return NoContent();
            }

            _logger.LogInfo(
                $"Returning {jobs.Count} jobs successfully for all periods for collection :{collectionName}");
            return Ok(jobs);
        }

        [HttpGet("retried/{collectionName}")]
        public async Task<IActionResult> GetRetriedJobs(string collectionName, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get retried jobs for the current period for collection {collectionName}");

            if (string.IsNullOrEmpty(collectionName))
            {
                return BadRequest("Empty collection name");
            }

            var period = await _returnCalendarService.GetCurrentPeriodAsync(collectionName);
            if (period == null)
            {
                return BadRequest($"No open period for collection {collectionName}");
            }

            var data = _jobQueueDataContext.IlrJobMetaData
                .FromSql("dbo.GetRetriedJobs @collectionName={0}, @period={1}", collectionName, period.PeriodNumber)
                .Select(x => x.JobId).Distinct().Count();

            _logger.LogInfo($"Returning retried job count : {data} for period :{period.PeriodNumber}, collection :{collectionName}");
            return Ok(data);
        }

        [HttpGet("{ukprn}/{collectionName}/latest")]
        public async Task<IActionResult> GetLatestJob(long ukprn, string collectionName, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get the with ukprn: {ukprn}, collection name :{collectionName}");

            if (ukprn == 0 || string.IsNullOrEmpty(collectionName))
            {
                _logger.LogWarning($"Request received with ukprn {ukprn}, collection name :{collectionName}, returning bad request");
                return BadRequest();
            }

            var job = await _jobQueryService.GetLatestJobByUkprnAsync(ukprn, collectionName, cancellationToken);

            _logger.LogInfo($"Returning job successfully for ukprn :{ukprn}");
            return Ok(job);
        }

        [HttpGet("{collectionName}/latest")]
        public async Task<IActionResult> GetLatestJobForCollection(string collectionName, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get the latest job for collection name :{collectionName}");

            if (string.IsNullOrEmpty(collectionName))
            {
                _logger.LogWarning($"Request received with collection name :{collectionName}, returning bad request");
                return BadRequest();
            }

            var job = await _jobQueryService.GetLatestJobByCollectionAsync(collectionName, cancellationToken);

            if (job == null)
            {
                return NoContent();
            }

            _logger.LogInfo($"Returning job successfully for collection :{collectionName}");
            return Ok(job);
        }

        [HttpGet("{collectionType}/latestByType")]
        public async Task<IActionResult> GetLatestSuccessfulJobByCollectionTypeByCollectioNameAsync(string collectionType, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get the latest successful job for collection type :{collectionType}");

            if (string.IsNullOrEmpty(collectionType))
            {
                _logger.LogWarning($"Request received with collection type :{collectionType}, returning bad request");
                return BadRequest();
            }

            var jobs = await _jobQueryService.GetLatestSuccessfulJobByCollectionTypeByCollectioNameAsync(collectionType, cancellationToken);

            if (jobs == null)
            {
                return NoContent();
            }

            _logger.LogInfo($"Returning jobs successfully for collection type :{collectionType}");
            return Ok(jobs);
        }

        [HttpGet("{ukprn}/{contractReference}/{collectionName}/latest")]
        public async Task<IActionResult> GetLatestJob(long ukprn, string contractReference, string collectionName, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get the with ukprn: {ukprn}, contract reference: {contractReference}, collection name :{collectionName}");

            if (ukprn == 0 || string.IsNullOrEmpty(collectionName) || string.IsNullOrEmpty(contractReference))
            {
                _logger.LogWarning($"Request received with ukprn {ukprn}, contract reference: {contractReference}, collection name :{collectionName}, returning bad request");
                return BadRequest();
            }

            var job = await _jobQueryService.GetLatestJobByUkprnAndContractReferenceAsync(ukprn, contractReference, collectionName, cancellationToken);

            _logger.LogInfo($"Returning job successfully for ukprn :{ukprn}, contract reference: {contractReference}, collection name :{collectionName}");
            return Ok(job);
        }

        [HttpGet("{jobId}/status")]
        public async Task<ActionResult> GetStatus(long jobId, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"GetJobStatus for jobId received {jobId}", jobIdOverride: jobId);

            if (jobId == 0)
            {
                _logger.LogWarning("Job Get status request received with empty data");
                return BadRequest();
            }

            try
            {
                var result = await _jobQueryService.GetJobById(jobId);
                if (result != null)
                {
                    _logger.LogInfo($"Successfully Got job for job Id: {jobId}", jobIdOverride: jobId);
                    return Ok(result.Status);
                }

                _logger.LogWarning($"Get status failed for job Id: {jobId}", jobIdOverride: jobId);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get status for job failed for jobId: {jobId}", ex);

                return BadRequest();
            }
        }

        [HttpGet("unsubmitted/{ukprn}")]
        public async Task<IActionResult> GetUnsubmittedIlrJobs(long ukprn, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get unsubmitted jobs for ukprn {ukprn}");

            if (ukprn <= 0)
            {
                return BadRequest("bad ukprn value");
            }

            var data = await _jobQueryService.GetUnsubmittedIlrJobsAsync(ukprn, cancellationToken);

            _logger.LogInfo($"Returning unsubmitted job count : {data.Count()} for ukprn :{ukprn}");
            return Ok(data);
        }

        [HttpGet("failedJobs/{collectionYear}/{period}")]
        public async Task<IActionResult> GetFailedJobsPerPeriod(int collectionYear, int period, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Request received to get failed jobs for year {collectionYear} and period {period}");

            var failedJobs = (await _jobQueryService.GetFailedJobsPerPeriodAsync(collectionYear, period, null, cancellationToken)).ToList();

            return Ok(failedJobs);
        }

        [HttpGet("jobsthatarequeued")]
        public async Task<ActionResult> GetJobsThatAreQueued(CancellationToken cancellationToken)
        {
            try
            {
                var data = await _jobProcessingService.GetJobsThatAreQueued(_dateTimeProvider.GetNowUtc());

                return new OkObjectResult(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("jobsthatareprocessing")]
        public async Task<ActionResult> GetJobsThatAreProcessing(CancellationToken cancellationToken)
        {
            try
            {
                var data = await _jobProcessingService.GetJobsThatAreProcessing(_dateTimeProvider.GetNowUtc());

                return new OkObjectResult(data);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("jobsthataresubmitted")]
        public async Task<ActionResult> GetJobsThatAreSubmitted(CancellationToken cancellationToken)
        {
            try
            {
                var data = await _jobProcessingService.GetJobsThatAreSubmitted(_dateTimeProvider.GetNowUtc());

                return new OkObjectResult(data);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("jobsthatarefailedtoday")]
        public async Task<ActionResult> GetJobsThatAreFailedToday(CancellationToken cancellationToken)
        {
            try
            {
                var data = await _jobProcessingService.GetJobsThatAreFailedToday(_dateTimeProvider.GetNowUtc());

                return new OkObjectResult(data);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("jobsthatareslowfile")]
        public async Task<ActionResult> GetJobsThatAreSlowFile(CancellationToken cancellationToken)
        {
            try
            {
                var data = await _jobProcessingService.GetJobsThatAreSlowFile(_dateTimeProvider.GetNowUtc());

                return new OkObjectResult(data);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("job-processing-details/{jobStatus}/{startDateTimeUtc}/{endDateTimeUtc}/{pageSize?}/{pageNumber?}")]
        public async Task<ActionResult> GetProcessingJobDetails(short jobStatus, DateTime startDateTimeUtc, DateTime endDateTimeUtc, int pageSize = int.MaxValue, int pageNumber = 1, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var data = await _jobProcessingService.GetPaginatedJobsInDetail(jobStatus, startDateTimeUtc, endDateTimeUtc, pageSize, pageNumber);

                if (data?.TotalItems > 0)
                {
                    Response.AddPaginationHeader(data);
                    return Ok(data.List);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured calling get processing job details for status : {jobStatus}, start date time : {startDateTimeUtc}, end date time ; {endDateTimeUtc}", ex);
                return BadRequest();
            }
        }

        [HttpGet("jobsthatareconcern")]
        public async Task<ActionResult> GetJobsThatAreConcern(CancellationToken cancellationToken)
        {
            try
            {
                var data = await _jobProcessingService.GetJobsThatAreConcern(_dateTimeProvider.GetNowUtc());

                return new OkObjectResult(data);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("jobsthatareadasmismatch/{collectionYear?}")]
        public async Task<ActionResult> GetJobsThatAreADasMismatchAsync(int? collectionYear, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _jobProcessingService.GetJobsThatAreADasMismatchAsync(collectionYear, cancellationToken);

                return new OkObjectResult(data);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("failedJobsCurrentPeriod")]
        public async Task<ActionResult> GetFailedJobsInCurrentPeriodAsync(CancellationToken cancellationToken)
        {
            var data = await _jobProcessingService.GetFailedJobsInCurrentPeriodAsync(cancellationToken);

            return new OkObjectResult(data);
        }

        [HttpGet("providersReturnedCurrentPeriod")]
        public async Task<ActionResult> GetProvidersReturnedCurrentPeriodAsync(CancellationToken cancellationToken)
        {
            var data = await _jobProcessingService.GetProvidersReturnedCurrentPeriodAsync(cancellationToken);

            return new OkObjectResult(data);
        }

        [HttpGet("job-processing-details/current-period/{jobStatus}/{pageSize?}/{pageNumber?}")]
        public async Task<ActionResult> GetProcessingJobDetails(short jobStatus, int pageSize = int.MaxValue, int pageNumber = 1, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var data = await _jobProcessingService.GetPaginatedJobsInDetailForCurrentOrClosedPeriod(jobStatus, pageSize, pageNumber);

                if (data?.TotalItems > 0)
                {
                    Response.AddPaginationHeader(data);
                    return Ok(data.List);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured calling get processing job details for period for status : {jobStatus}", ex);
                return BadRequest();
            }
        }

        [HttpGet("jobs-processing-details/current-period/{jobStatus}")]
        public async Task<ActionResult> GetJobsProcessingDetailsCurrentPeriodAsync(short jobStatus, CancellationToken cancellationToken)
        {
            var data = await _jobProcessingService.GetJobsProcessingCurrentPeriodAsync(jobStatus, _dateTimeProvider.GetNowUtc(), cancellationToken);

            return new OkObjectResult(data);
        }

        [HttpGet("jobs-processing-details/current-period/last5mins/{jobStatus}")]
        public async Task<ActionResult> GetJobsProcessingDetailsCurrentPeriodLast5MinsAsync(short jobStatus, CancellationToken cancellationToken)
        {
            var data = await _jobProcessingService.GetJobsProcessingCurrentPeriodLast5MinsAsync(jobStatus, _dateTimeProvider.GetNowUtc(), cancellationToken);

            return new OkObjectResult(data);
        }

        [HttpGet("jobs-processing-details/current-period/lasthour/{jobStatus}")]
        public async Task<ActionResult> GetJobsProcessingDetailsCurrentPeriodLastHourAsync(short jobStatus, CancellationToken cancellationToken)
        {
            var data = await _jobProcessingService.GetJobsProcessingCurrentPeriodLastHourAsync(jobStatus, _dateTimeProvider.GetNowUtc(), cancellationToken);

            return new OkObjectResult(data);
        }

        [HttpGet("job-submitted-collection-types/{ukprn}")]
        public async Task<ActionResult> GetCollectionTypesWithSubmissionsForProviderAsync(long ukprn, CancellationToken cancellationToken)
        {
            var data = await _jobQueryService.GetCollectionTypesWithSubmissionsForProviderAsync(ukprn, cancellationToken);

            return new OkObjectResult(data);
        }

        #endregion

        #region Post Calls

        [HttpPost("cross-loading/status/{jobId}/{status}")]
        public async Task<ActionResult> Post([FromRoute] long jobId, [FromRoute] JobStatusType status, CancellationToken cancellationToken)
        {
            if (jobId == 0)
            {
                _logger.LogWarning("Job Post request received with empty data");
                return BadRequest();
            }

            try
            {
                var job = await _jobQueryService.GetJobById(jobId);
                if (job == null)
                {
                    _logger.LogError($"JobId {jobId} is not valid for job status update", jobIdOverride: jobId);
                    return BadRequest("Invalid job Id");
                }

                var result = await _jobManager.UpdateCrossLoadingStatus(job.JobId, status, cancellationToken);

                if (result)
                {
                    _logger.LogInfo($"Successfully updated cross loading job status for job Id: {jobId}", jobIdOverride: jobId);
                    return Ok();
                }

                _logger.LogWarning($"Update cross loading status failed for job Id: {jobId}", jobIdOverride: jobId);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Post for cross loading status post job failed for job: {jobId}", ex);

                return BadRequest();
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("{status}")]
        public async Task<ActionResult> Post([FromBody] JobStatusDto jobStatusDto, CancellationToken cancellationToken)
        {
            if (jobStatusDto == null)
            {
                _logger.LogError($"Job Post request received with empty data for JobStatusDto");
                return BadRequest();
            }

            _logger.LogInfo("Post for job received for job: {@jobStatusDto} ", new[] { jobStatusDto });

            if (jobStatusDto.JobId == 0)
            {
                _logger.LogWarning("Job Post request received with empty data");
                return BadRequest();
            }

            if (!Enum.IsDefined(typeof(JobStatusType), jobStatusDto.JobStatus))
            {
                _logger.LogWarning($"Job Post request received with bad status {jobStatusDto.JobStatus}");
                return BadRequest("Status is not a valid value");
            }

            try
            {
                var job = await _jobQueryService.GetJobById(jobStatusDto.JobId);
                if (job == null)
                {
                    _logger.LogError($"JobId {jobStatusDto.JobId} is not valid for job status update", jobIdOverride: jobStatusDto.JobId);
                    return BadRequest("Invalid job Id");
                }

                var jobStatusType = (JobStatusType)jobStatusDto.JobStatus;

                // Check if it has come through with continue on failure
                if (job.CollectionType == CollectionTypeConstants.Ilr
                    && (jobStatusType == JobStatusType.FailedRetry || jobStatusType == JobStatusType.Failed)
                    && !jobStatusDto.ContinueToFailJob)
                {
                    _logger.LogInfo("ILR job failed, trying to send message to DAS", null, job.JobId);
                    await _failedJobNotificationService.SendMessageAsync(job);
                    return Ok();
                }

                // If we are changing from Waiting to Ready, it means processing should go to second stage
                if (job.Status == JobStatusType.Waiting && jobStatusType == JobStatusType.Ready)
                {
                    await _fileUploadJobManager.SubmitIlrJob(job.JobId);
                }

                bool result = await _jobManager.UpdateJobStatus(job.JobId, jobStatusType);

                if (jobStatusType == JobStatusType.Completed && (job.CollectionType == CollectionTypeConstants.PeriodEnd_ILR ||
                                                                 job.CollectionType == CollectionTypeConstants.PeriodEnd_NCS ||
                                                                 job.CollectionType == CollectionTypeConstants.PeriodEnd_ALLF))
                {
                    try
                    {
                        var path = await _periodEndStateService.GetPathforJob(jobStatusDto.JobId);
                        var pathItems = (await _periodEndStateService.GetPathItemsForPath(path.PathId, path.Year ?? 0, path.Period)).ToList();

                        var pathItem = pathItems.Single(pi => pi.PathItemJobs.Any(pij => pij.JobId == jobStatusDto.JobId));

                        if (!pathItem.IsPausing)
                        {
                            await _periodEndService.ProceedAsync(jobStatusDto.JobId, cancellationToken);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Failed in period end service", e);
                    }
                }

                if (result)
                {
                    _logger.LogInfo($"Successfully updated job status for job Id: {jobStatusDto.JobId} to {(JobStatusType)jobStatusDto.JobStatus}", jobIdOverride: jobStatusDto.JobId);

                    Func<JobUpdateDTO> func = () => BuildJobUpdateDTO((jobStatusDto));
                    var audit = _auditFactory.BuildRequestAudit<JobUpdateDTO>(func);
                    await audit.AfterAndSaveAsync(cancellationToken);

                    return Ok();
                }

                _logger.LogWarning($"Failed to Update job status for job Id: {jobStatusDto.JobId} to {(JobStatusType)jobStatusDto.JobStatus}");
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to Update job status for job Id: {jobStatusDto.JobId} to {(JobStatusType)jobStatusDto.JobStatus}", ex);

                return BadRequest();
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] FileUploadJob job, CancellationToken cancellationToken)
        {
            ActionResult postResult = await base.Post(job);

            Func<JobCreationDTO> func = () => BuildJobCreationDTO((job));
            var audit = _auditFactory.BuildRequestAudit<JobCreationDTO>(func);
            await audit.AfterAndSaveAsync(cancellationToken);

            return postResult;
        }

        [HttpPost("clone-job")]
        public async Task<ActionResult> CloneJob([FromBody] long jobId, CancellationToken cancellationToken)
        {
            if (jobId <= 0)
            {
                _logger.LogError($"Clone Job Post request received with empty invalid jobid");
                return BadRequest();
            }

            _logger.LogInfo($"CloneJob for job received for jobid: {jobId}");

            try
            {
                var newJobId = await _jobManager.CloneJob(jobId, cancellationToken);
                if (newJobId > 0)
                {
                    _logger.LogInfo($"Successfully created clone of job with id: {jobId} to new job with id : {newJobId}");

                    return Ok(newJobId);
                }
                else
                {
                    _logger.LogWarning($"Failed to clone job for job Id: {jobId}");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to clone job for job Id: {jobId}", ex);

                return BadRequest();
            }
        }

        #endregion

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Delete for job received for job id: {id}");

            if (id == 0)
            {
                return BadRequest();
            }

            try
            {
                await _jobManager.RemoveJobFromQueue(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete for job failed for job: {id}", ex);

                return BadRequest();
            }
        }

        private JobCreationDTO BuildJobCreationDTO(FileUploadJob job)
        {
            return new JobCreationDTO() { JobID = job.JobId, FileName = job.FileName };
        }

        private JobUpdateDTO BuildJobUpdateDTO(JobStatusDto job)
        {
            return new JobUpdateDTO() { JobID = job.JobId };
        }
    }
}
