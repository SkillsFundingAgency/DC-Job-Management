using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Audit.Models.DTOs.Job;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/job")]
    public class ValidationRuleDetailsReportController : BaseJobController<ValidationRuleDetailsReportJob>
    {
        private readonly IAuditFactory _auditFactory;

        public ValidationRuleDetailsReportController(
            IAuditFactory auditFactory,
            ILogger logger,
            Func<ValidationRuleDetailsReportJob> validationRuleDetailsReportFunc,
            IUpdateJobManager<ValidationRuleDetailsReportJob> validationRuleDetailsReportPublicationJobManager)
            : base(validationRuleDetailsReportPublicationJobManager, validationRuleDetailsReportFunc, logger)
        {
            _auditFactory = auditFactory;
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost("validationruledetailsreport")]
        public async Task<IActionResult> Post([FromBody] ValidationRuleDetailsReportJob job, CancellationToken cancellationToken)
        {
            ActionResult postResult = await base.Post(job);
            Func<JobCreationDTO> func = () => BuildJobCreationDTO(job);
            var audit = _auditFactory.BuildRequestAudit<JobCreationDTO>(func);
            await audit.AfterAndSaveAsync(cancellationToken);
            return postResult;
        }

        private JobCreationDTO BuildJobCreationDTO(ValidationRuleDetailsReportJob job)
        {
            return new JobCreationDTO() { JobID = job.JobId };
        }
    }
}