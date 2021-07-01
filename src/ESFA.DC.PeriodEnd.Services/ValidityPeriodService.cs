using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.PeriodEnd.Services
{
    public class ValidityPeriodService : IValidityPeriodService
    {
        private readonly IPeriodEndRepository _periodEndRepository;
        private readonly IQueryService _queryService;

        public ValidityPeriodService(
            IPeriodEndRepository periodEndRepository,
            IQueryService queryService,
            IJsonSerializationService jsonSerializationService)
        {
            _periodEndRepository = periodEndRepository;
            _queryService = queryService;
        }

        public async Task<IEnumerable<int>> GetValidityPeriodsForEmail(int hubEmailId)
        {
            return await _periodEndRepository.GetValidityPeriodsForEmailAsync(hubEmailId);
        }

        public async Task<IDictionary<string, IEnumerable<int>>> GetValidPeriodsForCollections(int collectionYear)
        {
            return await _periodEndRepository.GetValidPeriodsForCollectionsAndEmailsAsync(collectionYear);
        }

        public async Task UpdateValidityPeriods(int collectionYear, int period, List<ValidityPeriodModel> validityPeriods, CancellationToken cancellationToken)
        {
            await _queryService.UpdateValidityPeriods(collectionYear, period, validityPeriods);
        }

        public async Task<bool> HasPeriodEndRunForPeriodAsync(string collectionType, int collectionYear, int period, CancellationToken cancellationToken)
        {
            return await _periodEndRepository.HasPeriodEndRunForPeriodAsync(collectionType, collectionYear, period, cancellationToken);
        }
    }
}