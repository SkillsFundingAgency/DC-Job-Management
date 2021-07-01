using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.Jobs.Model;
using Job = ESFA.DC.JobQueueManager.Data.Entities.Job;

namespace ESFA.DC.JobQueueManager.JobsMetaDataManager
{
    public class EasJobMetaDataManager : IJobMetaDataManager
    {
        public void AddMetaData(Job jobEntity, FileUploadJob sourceJob, IJobQueueDataContext context)
        {
            if (sourceJob.TermsAccepted.HasValue)
            {
                context.EasJobMetaData.Add(new EasJobMetaData()
                {
                    TermsAccepted = sourceJob.TermsAccepted.Value,
                    Job = jobEntity
                });
            }
        }
    }
}
