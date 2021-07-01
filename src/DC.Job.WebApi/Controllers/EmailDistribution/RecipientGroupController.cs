using System;
using System.Threading.Tasks;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.EmailDistribution
{
    [Produces("application/json")]
    [Route("api/email-distribution/groups")]
    public class RecipientGroupController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmailDistributionService _emailDistributionService;

        public RecipientGroupController(
            ILogger logger,
            IEmailDistributionService emailDistributionService)
        {
            _logger = logger;
            _emailDistributionService = emailDistributionService;
        }

        [HttpGet("{recipientGroupId}")]
        public async Task<IActionResult> GetEmailDistributionGroup(int recipientGroupId)
        {
            _logger.LogDebug("In EmailDistributionController");

            try
            {
                var data = await _emailDistributionService.GetEmailDistributionGroup(recipientGroupId);
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in GetEmailDistributionGroups", e);
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmailDistributionGroups()
        {
            _logger.LogDebug("In EmailDistributionController");

            try
            {
                var data = await _emailDistributionService.GetEmailDistributionGroups();
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in GetEmailDistributionGroups", e);
                return BadRequest();
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost]
        public async Task<IActionResult> SaveRecipientGroup([FromBody] string groupName)
        {
            _logger.LogDebug("In EmailDistributionController");

            try
            {
                await _emailDistributionService.SaveRecipientGroup(groupName);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Error in GetEmailDistributionGroups", e);
                return BadRequest();
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveRecipientGroup([FromBody] int recipientGroupId)
        {
            _logger.LogDebug("In EmailDistributionController");

            try
            {
                var result = await _emailDistributionService.RemoveRecipientGroup(recipientGroupId);
                if (result)
                {
                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError("Error in GetEmailDistributionGroups", e);
                return BadRequest();
            }
        }
    }
}