using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Data.Models;

namespace ESFA.DC.Data.Services.Interfaces
{
    public interface ISummarisationDataService
    {
        Task<List<SummarisationCollectionReturnCode>> GetLatestSummarisationCollectionCodesAsync(string collectionType, CancellationToken cancellationToken, int? maxCollectionsCodesCount = null, DateTime? dateTimeUntil = null);

        Task<List<SummarisationCollectionReturnCode>> GetSummarisationCollectionCodesAsync(string collectionType, string collectionReturnCode, string previousCollectionReturnCode, CancellationToken cancellationToken);

        Task<List<SummarisationTotal>> GetSummarisationTotalsAsync(List<int> collectionReturnIds, CancellationToken cancellationToken);

        Task<List<SummarisationCollectionReturnCode>> GetReturnCodeForPeriodAsync(
            string collectionType,
            int year,
            int period,
            int pathItemId,
            CancellationToken cancellationToken);
    }
}
