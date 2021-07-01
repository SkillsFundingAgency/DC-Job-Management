using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IValidityPeriodService
    {
        Task<IEnumerable<int>> GetValidityPeriodsForEmail(int hubEmailId);

        Task<IDictionary<string, IEnumerable<int>>> GetValidPeriodsForCollections(int collectionYear);

        Task UpdateValidityPeriods(int collectionYear, int period, List<ValidityPeriodModel> validityPeriods, CancellationToken cancellationToken);

        Task<bool> HasPeriodEndRunForPeriodAsync(string collectionType, int collectionYear, int period, CancellationToken cancellationToken);
    }
}