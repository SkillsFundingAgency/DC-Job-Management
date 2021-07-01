using System.Threading.Tasks;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.Email;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.PeriodEnd
{
    [Produces("application/json")]
    [Route("api/period-end-email")]
    public class PeriodEndMailingListController : Controller
    {
        private readonly ILogger _logger;
        private readonly IPeriodEndEmailService _periodEndEmailService;

        public PeriodEndMailingListController(
            ILogger logger,
            IPeriodEndEmailService periodEndEmailService)
        {
            _logger = logger;
            _periodEndEmailService = periodEndEmailService;
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("{hubEmailId}/{period}/{periodprefix}")]
        public async Task<IActionResult> PeriodEndEmailAsync(int hubEmailId, int period, string periodprefix)
        {
            _logger.LogDebug($"In Period End Mailing List API PeriodEndEmailAsync method, emailId {hubEmailId}, period {period}, prefix {periodprefix}");

            await _periodEndEmailService.PeriodEndEmail(hubEmailId, period, periodprefix);
            return Ok();
        }
    }
}
