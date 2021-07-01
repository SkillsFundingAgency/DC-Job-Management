using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface INcsDssJobMetaDataService
    {
        Task<NcsDssJobMetaData> GetNcsDssJobParameters(long jobId);
    }
}
