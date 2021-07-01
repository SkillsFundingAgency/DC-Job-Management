using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces.DataAccess
{
    public interface IQueryService
    {
        Task ClearDownPeriodEnd(string collectionName, int period);

        Task<IEnumerable<ProviderJob>> GetNonSubmittingProviders(string collectionName, int periodNumber);

        Task<IEnumerable<ProviderJob>> GetSubmittingProviders(string collectionName);

        Task<IEnumerable<ProviderJob>> GetLatestSubmittedJobs(string collectionName);

        Task<IEnumerable<ProviderJob>> GetLatestDASSubmittedJobs(string ilrCollectionName, string dasSubmissionCollectionName);

        Task UpdateValidityPeriods(int collectionYear, int period, List<ValidityPeriodModel> validityPeriods);
    }
}