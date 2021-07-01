using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data.Entities;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IFisReferenceDataJobMetaDataService
    {
        Task<FisJobMetaData> GetFisJobMetaDataForJobId(long jobId, CancellationToken cancellationToken);

        Task<int> GetVersionNumberForJobId(long jobId, CancellationToken cancellationToken);

        Task SetGeneratedDateForJobId(long jobId, DateTime dateTime, CancellationToken cancellationToken);

        Task SetPublishedDateForJobId(long jobId, DateTime dateTime, CancellationToken cancellationToken);

        Task SetRemovedFlagForJobId(long jobId, CancellationToken cancellationToken);
    }
}
