using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.PeriodEnd
{
    public abstract class BaseReportController : Controller
    {
        private readonly IReportFileService _reportFileService;
        private readonly ICollectionService _collectionService;
        private readonly ILogger _logger;

        public BaseReportController(
            IReportFileService reportFileService,
            ICollectionService collectionService,
            ILogger logger)
        {
            _reportFileService = reportFileService;
            _logger = logger;
            _collectionService = collectionService;
        }

        [HttpGet("reports/{collectionYear}/{period}/{fileLocation}/{topReportCount?}")]
        public async Task<IActionResult> GetReportDetailsAsync(int collectionYear, int period, string fileLocation, CancellationToken cancellationToken, int topReportCount = 1)
        {
            if (string.IsNullOrEmpty(fileLocation))
            {
                string error = $"Missing '{nameof(fileLocation)}' parameter.";
                _logger.LogError(error);
                throw new Exception(error);
            }

            var model = await _reportFileService.GetReportDetails(fileLocation, period, cancellationToken, topReportCount);
            return Ok(model);
        }
    }
}