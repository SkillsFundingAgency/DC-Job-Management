using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IEsfService
    {
        Task UpdateJobMetaDataAsync(EsfJobMetaData esfJobMetaData, CancellationToken cancellationToken);

        Task CreateJobMetaDataAsync(EsfJobMetaData esfJobMetaData, CancellationToken cancellationToken);
    }
}
