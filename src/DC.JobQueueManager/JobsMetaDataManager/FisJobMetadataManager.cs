using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.Jobs.Model;
using Job = ESFA.DC.JobQueueManager.Data.Entities.Job;

namespace ESFA.DC.JobQueueManager.JobsMetaDataManager
{
    public class FisJobMetaDataManager : IJobMetaDataManager
    {
        public void AddMetaData(Job jobEntity, FileUploadJob sourceJob, IJobQueueDataContext context)
        {
            if (sourceJob.FisVersionNumber.HasValue)
            {
                context.FisJobMetaData.Add(new FisJobMetaData()
                {
                    VersionNumber = sourceJob.FisVersionNumber.Value,
                    Job = jobEntity
                });
            }
        }
    }
}
