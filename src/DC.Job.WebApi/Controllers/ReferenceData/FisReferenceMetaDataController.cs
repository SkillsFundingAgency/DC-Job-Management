using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.ReferenceData
{
    [Produces("application/json")]
    [Route("api/reference-data-fis-referencedata")]
    public class FisReferenceMetaDataController : Controller
    {
        private readonly IFisReferenceDataJobMetaDataService _referenceDataService;

        public FisReferenceMetaDataController(IFisReferenceDataJobMetaDataService referenceDataService)
        {
            _referenceDataService = referenceDataService;
        }

        [HttpGet("{jobId}")]
        public async Task<int> GetFisRefDataVersionAsync(long jobId, CancellationToken cancellationToken)
        {
            return await _referenceDataService.GetVersionNumberForJobId(jobId, cancellationToken);
        }

        [HttpPost("{jobId}/generation-{dateTime}")]
        public async Task SetFisRefDataGenerationDateForVersionAsync(long jobId, DateTime dateTime, CancellationToken cancellationToken)
        {
            await _referenceDataService.SetGeneratedDateForJobId(jobId, dateTime, cancellationToken);
        }

        [HttpPost("{jobId}/publish-{dateTime}")]
        public async Task SetFisRefDataPublishDateForVersionAsync(long jobId, DateTime dateTime, CancellationToken cancellationToken)
        {
            await _referenceDataService.SetPublishedDateForJobId(jobId, dateTime, cancellationToken);
        }

        [HttpPost("{jobId}/remove")]
        public async Task SetFisRefDataIsRemovedForVersionAsync(long jobId, CancellationToken cancellationToken)
        {
            await _referenceDataService.SetRemovedFlagForJobId(jobId, cancellationToken);
        }
    }
}