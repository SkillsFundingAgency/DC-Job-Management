using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IBaseJobManager<T>
        where T : Jobs.Model.Job
    {
        Task<long> AddJob(T job);

        Task<IEnumerable<T>> GetAllJobs();
    }
}
