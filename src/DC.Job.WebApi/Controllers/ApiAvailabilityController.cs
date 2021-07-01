using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/apiavailability")]
    public class ApiAvailabilityController : Controller
    {
        private readonly IApiAvailabilityService _apiAvailabilityService;
        private readonly ILogger _logger;

        public ApiAvailabilityController(
            IApiAvailabilityService apiAvailabilityService,
            ILogger logger)
        {
            _apiAvailabilityService = apiAvailabilityService;
            _logger = logger;
        }

        [HttpGet("{apiName}")]
        public async Task<ActionResult> GetAvailabilityAsync(string apiName, CancellationToken cancellationToken)
        {
            try
            {
                var apiAvailable = await _apiAvailabilityService.IsApiAvailableAsync(apiName, cancellationToken);
                if (!apiAvailable)
                {
                    return Ok(false);
                }

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting api availability.", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("set")]
        public async Task<ActionResult> SetApiAvailabilityAsync([FromBody] ApiAvailabilityDto apiAvailabilityDto, CancellationToken cancellationToken)
        {
            try
            {
                await _apiAvailabilityService.SetApiAvailabilityAsync(apiAvailabilityDto, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting api availability.", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}