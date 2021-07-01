using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.EmailDistribution
{
    [Produces("application/json")]
    [Route("api/email-distribution/recipients")]
    public class RecipientController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmailDistributionService _emailDistributionService;

        public RecipientController(
            ILogger logger,
            IEmailDistributionService emailDistributionService)
        {
            _logger = logger;
            _emailDistributionService = emailDistributionService;
        }

        [HttpGet("{recipientGroupId}")]
        public async Task<IActionResult> GetRecipients(int recipientGroupId)
        {
            _logger.LogDebug("In EmailDistributionController");

            try
            {
                var data = await _emailDistributionService.GetEmailDistributionGroupRecipients(recipientGroupId);
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
        public async Task<IActionResult> SaveAsync([FromBody] Recipient recipient, CancellationToken cancellationToken)
        {
            _logger.LogDebug("In RecipientController for save recipient");

            try
            {
                var saveRecipientResponse = await _emailDistributionService.SaveRecipientAsync(recipient, cancellationToken);
                if (saveRecipientResponse.IsDuplicateEmail)
                {
                    return StatusCode(StatusCodes.Status409Conflict, saveRecipientResponse.RecipientGroups);
                }

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Error in RecipientController save", e);
                return BadRequest();
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("remove/{recipientId}/{recipientGroupId}")]
        public async Task<IActionResult> RemoveRecipient(int recipientId, int recipientGroupId)
        {
            _logger.LogDebug("In RecipientController for save recipient");

            try
            {
                await _emailDistributionService.RemoveRecipient(recipientId, recipientGroupId);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Error in RecipientController save", e);
                return BadRequest();
            }
        }
    }
}