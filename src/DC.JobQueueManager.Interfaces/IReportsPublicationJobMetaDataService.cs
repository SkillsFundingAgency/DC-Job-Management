using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IReportsPublicationJobMetaDataService
    {
        Task<ReportsPublicationJobMetaData> GetFrmReportsJobParameters(long jobId);

        Task MarkAsPublishedAsync(long jobId, CancellationToken cancellationToken);

        Task MarkAsUnPublishedAsync(string collectionName, int period, CancellationToken cancellationToken);

        Task<IEnumerable<PublishedReportDto>> GetReportsPublicationDataAsync(CancellationToken cancellationToken, string collectionName = null);
    }
}
