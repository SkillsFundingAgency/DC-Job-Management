using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.PeriodEnd
{
    [Produces("application/json")]
    [Route("api/period-end-ncs")]
    public class ReportNCSController : BaseReportController
    {
        private readonly ILogger _logger;
        private readonly IReportFileServiceNCS _reportFileService;
        private readonly ICollectionService _collectionService;

        public ReportNCSController(
            ILogger logger,
            IReportFileServiceNCS reportFileService,
            ICollectionService collectionService)
            : base(reportFileService, collectionService, logger)
        {
            _logger = logger;
            _reportFileService = reportFileService;
            _collectionService = collectionService;
        }

        [HttpGet("reports/{collectionYear}/{period}")]
        public async Task<IActionResult> GetReportDetailsAsync(int collectionYear, int period, CancellationToken cancellationToken)
        {
            var periodEndContainerName = (
                    await _collectionService.GetCollectionFromNameAsync(
                        cancellationToken,
                        PeriodEndConstants.CollectionName_NCSDataExtractReport.Replace(PeriodEndConstants.CollectionNameYearToken, collectionYear.ToString())))
                .StorageReference;

            var model = await _reportFileService.GetReportDetails(periodEndContainerName, period, cancellationToken);

            return Ok(model);
        }
    }
}