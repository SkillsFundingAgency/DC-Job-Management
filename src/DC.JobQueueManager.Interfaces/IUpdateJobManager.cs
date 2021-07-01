using System.Threading.Tasks;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IUpdateJobManager<T> : IBaseJobManager<T>
        where T : Jobs.Model.Job
    {
        Task<bool> UpdateJob(T job);
    }
}
