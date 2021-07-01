using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface INcsJobManager : IBaseJobManager<NcsJob>
    {
        Task<NcsJob> GetJobById(long jobId);
    }
}
