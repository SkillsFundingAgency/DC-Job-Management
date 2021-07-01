using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager
{
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly IJobQueryService _jobQueryService;

        public ReferenceDataService(
            IJobQueryService jobQueryService)
        {
            _jobQueryService = jobQueryService;
        }

        public async Task<IEnumerable<JobMetaDataDto>> GetReferenceDataJobs(
            string collectionName,
            CancellationToken cancellationToken)
        {
            return await _jobQueryService.GetAllJobsPerCollectionAsync(collectionName, cancellationToken);
        }

        public async Task<IEnumerable<JobMetaDataDto>> GetFileUploadsForCollectionAsync(
            string collectionName,
            CancellationToken cancellationToken)
        {
            return await _jobQueryService.GetAllJobsPerCollectionAsync(collectionName, cancellationToken);
        }

        public async Task<IEnumerable<JobMetaDataDto>> GetPublishJobsForCollectionAsync(
            string collectionName,
            CancellationToken cancellationToken)
        {
            return await _jobQueryService.GetPublishJobsForCollectionAsync(collectionName, cancellationToken);
        }
    }
}