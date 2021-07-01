using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.ReferenceData
{
    [Produces("application/json")]
    [Route("api/reference-data-uploads")]
    public class ReferenceDataController : Controller
    {
        private readonly IReferenceDataService _referenceDataService;

        public ReferenceDataController(
            IReferenceDataService referenceDataService)
        {
            _referenceDataService = referenceDataService;
        }

        [HttpGet("reference-data-jobs/{collectionName}")]
        public async Task<IEnumerable<JobMetaDataDto>> GetReferenceDataJobs(string collectionName, CancellationToken cancellationToken)
        {
            return await _referenceDataService.GetReferenceDataJobs(collectionName, cancellationToken);
        }

        [HttpGet("file-uploads/{collectionName}")]
        public async Task<IEnumerable<JobMetaDataDto>> GetFileUploadsForPeriodAsync(string collectionName, CancellationToken cancellationToken)
        {
            return await _referenceDataService.GetFileUploadsForCollectionAsync(collectionName, cancellationToken);
        }

        [HttpGet("publish-jobs/{collectionName}")]
        public async Task<IEnumerable<JobMetaDataDto>> GetPublishJobsForCollectionAsync(string collectionName, CancellationToken cancellationToken)
        {
            return await _referenceDataService.GetPublishJobsForCollectionAsync(collectionName, cancellationToken);
        }
    }
}