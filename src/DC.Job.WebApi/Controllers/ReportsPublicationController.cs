using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Audit.Models.DTOs.FRM;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/job/publication")]
    public class ReportsPublicationController : BaseJobController<ReportsPublicationJob>
    {
        private readonly IReportsPublicationJobMetaDataService _reportsPublicationJobMetaDataService;
        private readonly IAuditFactory _auditFactory;

        public ReportsPublicationController(
            ILogger logger,
            Func<ReportsPublicationJob> reportsPublicationFunc,
            IUpdateJobManager<ReportsPublicationJob> reportsPublicationJobManager,
            IReportsPublicationJobMetaDataService reportsPublicationJobMetaDataService,
            IAuditFactory auditFactory)
            : base(reportsPublicationJobManager, reportsPublicationFunc, logger)
        {
            _reportsPublicationJobMetaDataService = reportsPublicationJobMetaDataService;
            _auditFactory = auditFactory;
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("validate")]
        public async Task<IActionResult> Post([FromBody] ReportsPublicationJob publicationJob, CancellationToken cancellationToken)
        {
            Func<FRMValidateDTO> func = () => buildFRMValidationDTO(publicationJob);
            var audit = _auditFactory.BuildRequestAudit(func);
            var result = await Post(publicationJob);
            await audit.AfterAndSaveAsync(cancellationToken);
            return result;
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("mark-as-published/{jobId}")]
        public async Task<IActionResult> PublishReportsAsync(long jobId, CancellationToken cancellationToken)
        {
            await _reportsPublicationJobMetaDataService.MarkAsPublishedAsync(jobId, cancellationToken);
            return Ok();
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("mark-as-unpublished/{collectionName}/{period}")]
        public async Task<IActionResult> UnpublishFrmReportsAsync(string collectionName, int period, CancellationToken cancellationToken)
        {
            await _reportsPublicationJobMetaDataService.MarkAsUnPublishedAsync(collectionName, period, cancellationToken);
            return Ok();
        }

        [HttpGet("published-periods/{collectionName?}")]
        public async Task<IActionResult> GetReportPublicationReportsData(CancellationToken cancellationToken, string collectionName = null)
        {
            var model = await _reportsPublicationJobMetaDataService.GetReportsPublicationDataAsync(cancellationToken, collectionName);
            return Ok(model);
        }

        private FRMValidateDTO buildFRMValidationDTO(ReportsPublicationJob publicationJob)
        {
            return new FRMValidateDTO()
            {
                CurrentContainerName = publicationJob.StorageReference,
                FrmContainerName = publicationJob.SourceContainerName,
                FrmPeriodNumber = publicationJob.PeriodNumber,
                FrmFolderKey = publicationJob.SourceFolderKey
            };
        }
    }
}