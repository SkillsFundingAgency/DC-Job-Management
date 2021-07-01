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
    [Route("api/period-end")]
    public class ReportILRController : BaseReportController
    {
        private readonly IReportFileServiceILR _reportFileService;
        private readonly ICollectionService _collectionService;

        public ReportILRController(
            ILogger logger,
            IReportFileServiceILR reportFileService,
            ICollectionService collectionService)
            : base(reportFileService, collectionService, logger)
        {
            _reportFileService = reportFileService;
            _collectionService = collectionService;
        }

        [HttpGet("mca-reports/{collectionYear}/{period}")]
        public async Task<IActionResult> GetMcaReportDetailsAsync(int collectionYear, int period, CancellationToken cancellationToken)
        {
            var periodEndContainerName = (await _collectionService
                    .GetCollectionFromNameAsync(
                        cancellationToken,
                        GetYearSpecificCollectionName(PeriodEndConstants.CollectionName_McaReport, collectionYear)))
                .StorageReference;

            var model = await _reportFileService.GetMcaReports(periodEndContainerName, collectionYear, period, CancellationToken.None);

            return Ok(model);
        }

        [HttpGet("reports/{collectionYear}/{period}/samples")]
        public async Task<IActionResult> GetSampleReportsAsync(int collectionYear, int period, CancellationToken cancellationToken)
        {
            var periodEndContainerName = (await _collectionService
                .GetCollectionFromNameAsync(
                    cancellationToken,
                    GetYearSpecificCollectionName(PeriodEndConstants.CollectionName_DasPeriodEndReports, collectionYear)))
                .StorageReference;

            var model = await _reportFileService.GetReportSamples(periodEndContainerName, period, CancellationToken.None);

            return Ok(model);
        }

        [HttpGet("reports/{collectionYear}/{period}/llvsamples")]
        public async Task<IActionResult> GetLLVSampleReportsAsync(int collectionYear, int period, CancellationToken cancellationToken)
        {
            var periodEndContainerName = (await _collectionService
                .GetCollectionFromNameAsync(
                    cancellationToken,
                    GetYearSpecificCollectionName(PeriodEndConstants.CollectionName_LLVReport, collectionYear)))
                .StorageReference;

            var model = await _reportFileService.GetLLVReportSamples(periodEndContainerName, period, CancellationToken.None);

            return Ok(model);
        }

        [HttpGet("reports/{collectionYear}/{period}")]
        public async Task<IActionResult> GetReportDetailsAsync(int collectionYear, int period, CancellationToken cancellationToken)
        {
            var periodEndContainerName = (
                    await _collectionService.GetCollectionFromNameAsync(
                        cancellationToken,
                        PeriodEndConstants.CollectionName_DasPeriodEndReports.Replace(PeriodEndConstants.CollectionNameYearToken, collectionYear.ToString())))
                .StorageReference;

            var model = await _reportFileService.GetReportDetails(periodEndContainerName, period, cancellationToken);

            return Ok(model);
        }

        private string GetYearSpecificCollectionName(string collectionName, int collectionYear)
        {
            return collectionName.Replace(PeriodEndConstants.CollectionNameYearToken, collectionYear.ToString());
        }
    }
}