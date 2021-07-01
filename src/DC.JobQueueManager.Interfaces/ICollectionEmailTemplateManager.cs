using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface ICollectionEmailTemplateManager
    {
        Task<string> GetTemplate(int collectionId, JobStatusType? jobStatus = null);
    }
}
