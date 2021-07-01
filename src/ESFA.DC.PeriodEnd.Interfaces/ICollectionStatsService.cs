using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface ICollectionStatsService
    {
        Task<IEnumerable<CollectionStats>> GetCollectionStats(
            string container,
            int period,
            CancellationToken cancellationToken);
    }
}