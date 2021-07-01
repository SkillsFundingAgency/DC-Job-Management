using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IJobManager : IUpdateJobManager<Jobs.Model.Job>
    {
        Task<IEnumerable<Jobs.Model.Job>> GetJobsByPriorityAsync(int resultCount);

        Task RemoveJobFromQueue(long jobId);

        Task<bool> UpdateJobStatus(long jobId, JobStatusType status);

        Task<bool> IsCrossLoadingEnabled(int collectionId);

        Task<bool> UpdateCrossLoadingStatus(long jobId, JobStatusType status, CancellationToken cancellationToken);

        Task SendEmailNotification(SubmittedJob job);

        Task<long> CloneJob(long jobId, CancellationToken cancellationToken);
    }
}