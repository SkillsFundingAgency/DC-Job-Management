using System;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.EmailDistribution
{
    [Produces("application/json")]
    [Route("api/email-distribution/templates")]
    public class EmailTemplateController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmailDistributionService _emailDistributionService;

        public EmailTemplateController(
            ILogger logger,
            IEmailDistributionService emailDistributionService)
        {
            _logger = logger;
            _emailDistributionService = emailDistributionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmailTemplates()
        {
            _logger.LogDebug("In GetEmailTemplates");

            try
            {
                var data = await _emailDistributionService.GetEmailTemplates();
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in GetEmailTemplates", e);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("{emailId}")]
        public async Task<IActionResult> GetEmailTemplate(int emailId)
        {
            _logger.LogDebug("In GetEmailTemplate");

            try
            {
                var data = await _emailDistributionService.GetEmailTemplate(emailId);
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in GetEmailTemplates", e);
                return BadRequest();
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] EmailTemplate template)
        {
            _logger.LogDebug("In EmailTemplateController for save template");

            try
            {
                await _emailDistributionService.SaveTemplate(template);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError("Error in EmailTemplateController save", e);
                return BadRequest();
            }
        }
    }
}