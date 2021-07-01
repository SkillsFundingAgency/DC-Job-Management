using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.Jobs.Model;
using EsfJobMetaData = ESFA.DC.JobQueueManager.Data.Entities.EsfJobMetaData;
using Job = ESFA.DC.JobQueueManager.Data.Entities.Job;

namespace ESFA.DC.JobQueueManager.JobsMetaDataManager
{
    public class EsfJobMetaDataManager : IJobMetaDataManager
    {
        public void AddMetaData(Job jobEntity, FileUploadJob sourceJob, IJobQueueDataContext context)
        {
            if (!string.IsNullOrEmpty(sourceJob.ContractReferenceNumber))
            {
                context.EsfJobMetaData.Add(new EsfJobMetaData()
                {
                    Job = jobEntity,
                    ContractReferenceNumber = sourceJob.ContractReferenceNumber
                });
            }
        }
    }
}
