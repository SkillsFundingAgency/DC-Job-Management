using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Converters;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/job/reference-data")]
    public class ReferenceDataJobsController : Controller
    {
        private readonly IJobQueueDataContext _jobQueueDataContext;
        private readonly IJobManager _jobManager;
        private readonly ILogger _logger;
        private readonly IJobConverter _jobConverter;

        public ReferenceDataJobsController(IJobQueueDataContext jobQueueDataContext, IJobManager jobManager, ILogger logger, IJobConverter jobConverter)
        {
            _jobQueueDataContext = jobQueueDataContext;
            _jobManager = jobManager;
            _logger = logger;
            _jobConverter = jobConverter;
        }

        // GET api/values/5
        [HttpGet("{count?}")]
        public async Task<IActionResult> Get(int count = 20)
        {
            var data = _jobQueueDataContext.Job.Include(x => x.Collection)
                .Where(x => x.Collection.CollectionTypeId == 3)
                .OrderByDescending(x => x.JobId)
                .Take(count);

            var result = new List<Jobs.Model.Job>();
            foreach (var jobEntity in data)
            {
                result.Add(await _jobConverter.Convert(jobEntity));
            }

            return Ok(result);
        }

        // GET api/values/5
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Jobs.Model.Job job)
        {
            _logger.LogInfo("Reference data Post for job received for job: {@job}", new object[] { job });
            if (job == null)
            {
                _logger.LogWarning("Reference data Job Post request received with empty data");
                return BadRequest();
            }

            if (!Enum.IsDefined(typeof(Jobs.Model.Enums.JobStatusType), job.Status))
            {
                _logger.LogWarning($"Reference data Job Post request received with bad status {job.Status}");
                return BadRequest("Status is not a valid value");
            }

            if (string.IsNullOrEmpty(job.CollectionName))
            {
                _logger.LogWarning($"Reference data Job Post request received with bad collection {job.CollectionName}");
                return BadRequest("collection name is not a valid value");
            }

            try
            {
                _logger.LogInfo($"Create Job request received with object: {job} ");

                job.JobId = await _jobManager.AddJob(job);

                if (job.JobId > 0)
                {
                    _logger.LogInfo($"Created job successfully with Id: {job.JobId}");
                    return Ok(job.JobId);
                }

                _logger.LogInfo("Create job failed for job: {@job}", new object[] { job });
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("Post for job failed for job: {@job}", ex, new object[] { job });

                return BadRequest();
            }
        }
    }
}