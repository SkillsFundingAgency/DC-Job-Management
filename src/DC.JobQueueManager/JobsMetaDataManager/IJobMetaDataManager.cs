using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.JobsMetaDataManager
{
    public interface IJobMetaDataManager
    {
        void AddMetaData(Data.Entities.Job jobEntity, FileUploadJob sourceJob, IJobQueueDataContext context);
    }
}
