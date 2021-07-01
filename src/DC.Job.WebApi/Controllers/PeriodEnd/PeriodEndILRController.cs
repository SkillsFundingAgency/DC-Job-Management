using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.PeriodEnd
{
    [Produces("application/json")]
    [Route("api/period-end")]
    public class PeriodEndILRController : BasePeriodEndController
    {
        private readonly ILogger _logger;
        private readonly IPeriodEndServiceILR _periodEndService;
        private readonly ICollectionStatsService _collectionStatsService;
        private readonly IPeriodEndOutputService _periodEndOutputService;
        private readonly ICollectionService _collectionService;

        public PeriodEndILRController(
            ILogger logger,
            IPeriodEndServiceILR periodEndService,
            ICollectionStatsService collectionStatsService,
            IPeriodEndOutputService periodEndOutputService,
            ICollectionService collectionService)
        : base(logger, periodEndService)
        {
            _logger = logger;
            _periodEndService = periodEndService;
            _collectionStatsService = collectionStatsService;
            _periodEndOutputService = periodEndOutputService;
            _collectionService = collectionService;
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("{collectionYear}/{period}/{collectionType}/close")]
        public override async Task<IActionResult> ClosePeriodEndAsync(int collectionYear, int period, string collectionType, CancellationToken cancellationToken)
        {
            var paused = await _periodEndService.ClosePeriodEndAsync(collectionYear, period, collectionType, cancellationToken);
            return Ok(paused);
        }

        [HttpPost("reference-data-jobs/{collectionYear}/{period}/{pause}")]
        public async Task<IActionResult> ToggleReferenceDataJobsAsync(int collectionYear, int period, bool pause, CancellationToken cancellationToken)
        {
            await _periodEndService.ToggleReferenceDataJobsAsync(pause, cancellationToken);
            return Ok();
        }

        [HttpGet("collectionstats/{collectionYear}/{period}")]
        public async Task<IActionResult> GetCollectionStatsAsync(int collectionYear, int period, CancellationToken cancellationToken)
        {
            var periodEndContainerName = (
                await _collectionService.GetCollectionFromNameAsync(
                    cancellationToken,
                    PeriodEndConstants.CollectionName_DasPeriodEndReports.Replace(PeriodEndConstants.CollectionNameYearToken, collectionYear.ToString())))
                .StorageReference;

            var model = await _collectionStatsService.GetCollectionStats(periodEndContainerName, period, cancellationToken);
            return Ok(model);
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("provider-reports/{collectionYear}/{period}/{collectionType}/publish")]
        public async Task<IActionResult> PublishProviderReportsAsync(int collectionYear, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _periodEndOutputService.PublishProviderReports(collectionYear, period, collectionType);
            return Ok();
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("fm36-reports/{collectionYear}/{period}/{collectionType}/publish")]
        public async Task<IActionResult> PublishFm36ReportsAsync(int collectionYear, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _periodEndOutputService.PublishFm36Reports(collectionYear, period, collectionType);
            return Ok();
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("mca-reports/{collectionYear}/{period}/{collectionType}/publish")]
        public async Task<IActionResult> PublishMcaReportsAsync(int collectionYear, int period, string collectionType, CancellationToken cancellationToken)
        {
            await _periodEndOutputService.PublishMcaReports(collectionYear, period, collectionType);
            return Ok();
        }

        [HttpGet("published-report-periods")]
        public async Task<IActionResult> GetPublishedReportPeriodsAsync(CancellationToken cancellationToken)
        {
            var data = await _periodEndService.GetPublishedReportPeriodsAsync(cancellationToken);
            return Ok(data);
        }
    }
}