using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IReferenceDataService
    {
        Task<IEnumerable<JobMetaDataDto>> GetReferenceDataJobs(
            string collectionName,
            CancellationToken cancellationToken);

        Task<IEnumerable<JobMetaDataDto>> GetFileUploadsForCollectionAsync(
            string collectionName,
            CancellationToken cancellationToken);

        Task<IEnumerable<JobMetaDataDto>> GetPublishJobsForCollectionAsync(
            string collectionName,
            CancellationToken cancellationToken);
    }
}