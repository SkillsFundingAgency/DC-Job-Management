using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces.DataAccess
{
    public interface IValidityPeriodRepository
    {
        Task<List<ValidityPeriodLookupModel>> GetValidityPeriodList(CancellationToken cancellationToken);

        Task<ValidityPeriodLookupModel> GetValidityPeriod(int collectionId, int collectionYear, int period, CancellationToken cancellationToken);

        Task<int> UpdateValidityPeriod(ValidityPeriodLookupModel validityPeriodLookupModel, CancellationToken cancellationToken);

        Task<List<ValidityPeriodLookupModel>> GetValidityPeriodList(int collectionYear, int period, CancellationToken cancellationToken);

        Task<Dictionary<int, ValidityPeriodLookupModel>> GetEmailValidities(int collectionYear, int period, CancellationToken cancellationToken);

        Task<Dictionary<int, SubPathValidityPeriodLookupModel>> GetPathValidityPeriods(int collectionYear, int period, CancellationToken cancellationToken);

        Task UpdateSubPathValidityPeriods(
            List<SubPathValidityPeriodLookupModel> subPathValidityPeriods,
            CancellationToken cancellationToken);
    }
}