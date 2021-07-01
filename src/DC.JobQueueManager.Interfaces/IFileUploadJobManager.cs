using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IFileUploadJobManager : IUpdateJobManager<FileUploadJob>
    {
        Task<bool> SubmitIlrJob(long jobId);
    }
}