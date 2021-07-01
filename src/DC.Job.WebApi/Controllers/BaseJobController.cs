using System;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    public abstract class BaseJobController<T> : ControllerBase
        where T : Jobs.Model.Job
    {
        private readonly IUpdateJobManager<T> _publicationJobManager;
        private readonly ILogger _logger;
        private readonly Func<T> _func;

        public BaseJobController(IUpdateJobManager<T> publicationJobManager, Func<T> func, ILogger logger)
        {
            _publicationJobManager = publicationJobManager;
            _logger = logger;
            _func = func;
        }

        public async Task<ActionResult> Post([FromBody] T job)
        {
            _logger.LogInfo("Post for job received for job: {@job}", new[] { job });
            if (job == null)
            {
                _logger.LogWarning("Job Post request received with empty data");
                return BadRequest();
            }

            if (!Enum.IsDefined(typeof(JobStatusType), job.Status))
            {
                _logger.LogWarning($"Job Post request received with bad status {job.Status}");
                return BadRequest("Status is not a valid value");
            }

            if (string.IsNullOrEmpty(job.CollectionName))
            {
                _logger.LogWarning($"Job Post request received with bad collection {job.CollectionName}");
                return BadRequest("collection id is not a valid value");
            }

            try
            {
                var jobModel = _func.Invoke();

                jobModel.Status = job.Status;
                jobModel.JobId = job.JobId;
                jobModel.CollectionName = job.CollectionName;
                jobModel.Priority = job.Priority;
                jobModel.CreatedBy = job.CreatedBy;
                jobModel.NotifyEmail = job.NotifyEmail;
                jobModel.DateTimeCreatedUtc = job.DateTimeCreatedUtc;

                if (job.JobId > 0)
                {
                    if (job.Status == JobStatusType.Ready || job.Status == JobStatusType.Paused ||
                        job.Status == JobStatusType.FailedRetry)
                    {
                        _logger.LogInfo($"Going to update job with job Id: {job.JobId}");

                        var result = await _publicationJobManager.UpdateJob(jobModel);
                        if (result)
                        {
                            _logger.LogInfo($"Successfully updated job with job Id: {job.JobId}");
                            return Ok();
                        }

                        _logger.LogWarning($"Update job failed for job Id: {job.JobId}");
                        return BadRequest();
                    }

                    _logger.LogWarning($"Update job rejected because job status is not updateable for job Id: {job.JobId}; status: {job.Status}");
                    return BadRequest($"Only job with status of {nameof(JobStatusType.Ready)}, {nameof(JobStatusType.Paused)} or {nameof(JobStatusType.FailedRetry)} can be updated.");
                }

                _logger.LogInfo($"Create Job request received with object: {job} ");

                job.JobId = await _publicationJobManager.AddJob(job);

                if (job.JobId > 0)
                {
                    _logger.LogInfo($"Created job successfully with Id: {job.JobId}");
                    return Ok(job.JobId);
                }

                _logger.LogInfo("Create job failed for job: {@job}", new[] { job });
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("Post for job failed for job: {@job}", ex, new[] { job });

                return BadRequest();
            }
        }
    }
}
