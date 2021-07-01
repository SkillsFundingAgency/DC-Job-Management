using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface ICollectionService
    {
        Task<Collection> GetCollectionAsync(CancellationToken cancellationToken, string collectionType);

        Task<Collection> GetCollectionAsync(CancellationToken cancellationToken, int id);

        Task<Collection> GetCollectionFromNameAsync(CancellationToken cancellationToken, string collectionName);

        Task<IEnumerable<Collection>> GetCollectionsByYearAsync(CancellationToken cancellationToken, int collectionYear);

        Task<IEnumerable<Collection>> GetAllCollectionsByYearAsync(CancellationToken cancellationToken, int collectionYear);

        Task<IEnumerable<int>> GetAcademicYearsAsync(CancellationToken cancellationToken, DateTime? dateTimeUtc = null, bool? includeClosed = false);

        Task<IEnumerable<int>> GetAvailableCollectionYearsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<int>> GetCollectionYearsByTypeAsync(CancellationToken cancellationToken, string collectionType);

        Task<IEnumerable<Collection>> GetCollectionsByTypeAsync(string collectionType, CancellationToken cancellationToken);

        Task<bool> UpdateCollectionProcessingOverrideStatusAsync(CancellationToken cancellationToken, int collectionId, bool? processingOverrideFlag);

        Task<IEnumerable<Collection>> GetAllCollectionTypesAsync(CancellationToken cancellationToken);

        Task<Collection> GetCollectionByDateAsync(CancellationToken cancellationToken, string collectionType, DateTime dateTimeUtc);

        Task<IEnumerable<Collection>> GetOpenCollectionsByDateRangeAsync(DateTime startDateTimeUtc, DateTime endDateTimeUtc, CancellationToken cancellationToken);

        Task<List<Collection>> GetCollectionsFromNamesAsync(CancellationToken cancellationToken, List<string> collectionNames);

        Task<IEnumerable<CollectionRelatedLink>> GetRelatedLinksAsync(CancellationToken cancellationToken, string collectionName);

        Task<DateTime?> GetCollectionStartDateAsync(string collectionName, CancellationToken cancellationToken);

        Task<bool> IsCollectionOpenByIdWithVarianceAsync(int id, DateTime now, int negativeVarianceInMonths, int positiveVarianceInMonths, CancellationToken cancellationToken);

        Task<int> GetCountOfProvidersForCollectionAsync(int collectionId, CancellationToken cancellationToken);

        Task<List<int>> GetApplicableCollectionYearsByTypeAsync(string collectionType, DateTime dateTime, CancellationToken cancellationToken);
    }
}
