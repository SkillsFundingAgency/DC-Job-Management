using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Converters
{
    public interface IJobConverter
    {
        Task<Data.Entities.Job> Convert(Jobs.Model.Job source);

        Task<Jobs.Model.Job> Convert(Data.Entities.Job source);

        Task Convert(Jobs.Model.Job source, Data.Entities.Job destination);

        Task Convert(Data.Entities.Job source, Jobs.Model.Job destination);

        Task Convert(Data.Entities.Job source, FileUploadJob destination);

        Task Convert(FileUploadJobMetaData source, FileUploadJob destination);

        Task Convert(NcsJobMetaData source, NcsJob destination);

        Task Convert(FileUploadJobMetaData source, Provider providerSource, FileUploadJob destination);
    }
}