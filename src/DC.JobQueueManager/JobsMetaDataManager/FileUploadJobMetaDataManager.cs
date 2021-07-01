using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.Jobs.Model;
using Job = ESFA.DC.JobQueueManager.Data.Entities.Job;

namespace ESFA.DC.JobQueueManager.JobsMetaDataManager
{
    public class FileUploadJobMetaDataManager : IJobMetaDataManager
    {
        public void AddMetaData(Job jobEntity, FileUploadJob sourceJob, IJobQueueDataContext context)
        {
            var metaEntity = new FileUploadJobMetaData()
            {
                Job = jobEntity,
                FileName = sourceJob.FileName,
                FileSize = sourceJob.FileSize,
                StorageReference = sourceJob.StorageReference,
                PeriodNumber = sourceJob.PeriodNumber,
            };

            context.FileUploadJobMetaData.Add(metaEntity);
        }
    }
}
