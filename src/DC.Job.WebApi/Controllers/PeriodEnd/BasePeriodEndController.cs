using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.PeriodEnd
{
    public abstract class BasePeriodEndController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IPeriodEndService _periodEndService;

        public BasePeriodEndController(
            ILogger logger,
            IPeriodEndService periodEndService)
        {
            _logger = logger;
            _periodEndService = periodEndService;
        }

        [HttpPost("{collectionYear}/{period}/{pathId}/proceed")]
        public async Task<IActionResult> ProceedAsync(int collectionYear, int period, int pathId, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"In Period End API proceed method from UI, collection year {collectionYear}, period {period}, path {pathId} ");

            await _periodEndService.ProceedAsync(collectionYear, period, pathId, cancellationToken);
            return Ok();
        }

        [HttpPost("{collectionYear}/{period}/{collectionType}/initialise")]
        public async Task<IActionResult> InitialisePeriodEndAsync(int collectionYear, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _periodEndService.InitialisePeriodEndAsync(collectionYear, period, collectionType, cancellationToken);
            return Ok();
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("{collectionYear}/{period}/{collectionType}/start")]
        public async Task<IActionResult> StartPeriodEndAsync(int collectionYear, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _periodEndService.StartPeriodEndAsync(collectionYear, period, collectionType, cancellationToken);
            return Ok();
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("{collectionYear}/{period}/{collectionType}/collection-closed")]
        public async Task<IActionResult> CollectionClosedEmailSentAsync(int collectionYear, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _periodEndService.CollectionClosedEmailSentAsync(collectionYear, period, collectionType, cancellationToken);
            return Ok();
        }

        [HttpGet("states-prep/{collectionType}/{collectionYear?}/{period?}")]
        public async Task<IActionResult> GetStatesPrepAsync(string collectionType, int? collectionYear, int? period, CancellationToken cancellationToken)
        {
            var model = await _periodEndService.GetPrepStateAsync(collectionYear, period, collectionType, cancellationToken);
            return Ok(model);
        }

        [HttpGet("states-main/{collectionType}/{collectionYear?}/{period?}")]
        public async Task<IActionResult> GetStatesMainAsync(string collectionType, int? collectionYear, int? period, CancellationToken cancellationToken)
        {
            var model = await _periodEndService.GetPathsStateAsync(collectionYear, period, collectionType, cancellationToken);
            return Ok(model);
        }

        [HttpPost("{jobId}/proceed")]
        public async Task<IActionResult> ProceedAsync(int jobId, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"In Period End API proceed method from job callback with jobid {jobId}");

            await _periodEndService.ProceedAsync(jobId);
            return Ok();
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("{collectionYear}/{period}/{collectionType}/close")]
        public virtual async Task<IActionResult> ClosePeriodEndAsync(int collectionYear, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _periodEndService.ClosePeriodEndAsync(collectionYear, period, collectionType, cancellationToken);
            return Ok();
        }
    }
}