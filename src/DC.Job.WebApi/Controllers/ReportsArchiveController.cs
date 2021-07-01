using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/reports-archive")]
    public class ReportsArchiveController : Controller
    {
        private readonly IReportsArchiveService _reportsArchiveService;

        public ReportsArchiveController(IReportsArchiveService reportsArchiveService)
        {
            _reportsArchiveService = reportsArchiveService;
        }

        [HttpGet("{ukprn}")]
        public async Task<List<ReportsArchive>> GetAll(CancellationToken cancellationToken, long ukprn)
        {
            return await _reportsArchiveService.GetAllReportsArchivesAsync(cancellationToken, ukprn);
        }
    }
}
