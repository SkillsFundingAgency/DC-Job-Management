using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IJobManagementService
    {
        Task<IEnumerable<FileUploadJob>> GetJobsForAllPeriods(string collectionName, short? statusCode, CancellationToken cancellationToken);
    }
}
