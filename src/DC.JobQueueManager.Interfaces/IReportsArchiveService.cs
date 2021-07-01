using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IReportsArchiveService
    {
        Task<List<ReportsArchive>> GetAllReportsArchivesAsync(CancellationToken cancellationToken, long ukprn);

        Task<IEnumerable<long>> GetProvidersWithDataAsync(CancellationToken cancellationToken, IEnumerable<long> ukprns);
    }
}
