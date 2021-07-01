using System;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.Ncs
{
    [Produces("application/json")]
    [Route("api/job/ncs")]
    public class NcsController : Controller
    {
        private readonly ILogger _logger;
        private readonly INcsJobManager _ncsJobManager;

        public NcsController(
            ILogger logger,
            INcsJobManager ncsJobManager)
        {
            _logger = logger;
            _ncsJobManager = ncsJobManager;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] NcsJob job)
        {
            _logger.LogInfo($"Post for job received for job: {job.JobId}");

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
                _logger.LogInfo($"Create Job request received with object: {job.JobId} ");

                job.JobId = await _ncsJobManager.AddJob(job);

                if (job.JobId > 0)
                {
                    _logger.LogInfo($"Created job successfully with Id: {job.JobId}");
                    return Ok(job.JobId);
                }

                _logger.LogInfo($"Create job failed for job: {job.JobId}");
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Post for job failed for job: {job.JobId}", ex);

                return BadRequest();
            }
        }
    }
}
