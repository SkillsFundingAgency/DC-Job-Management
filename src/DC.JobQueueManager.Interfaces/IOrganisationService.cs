using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IOrganisationService
    {
        Task<Organisation> GetByUkprn(long ukprn);

        Task<IEnumerable<CollectionType>> GetAvailableCollectionTypesAsync(long ukprn);

        Task<IEnumerable<Collection>> GetAvailableCollectionsAsync(long ukprn, string collectionType = null);

        Task<Collection> GetCollectionAsync(long ukprn, string collectionName);

        Task<OrganisationAttributes> GetOrganisationAttributes(long ukprn);

        Task<bool> AddOrganisation(Organisation organisation, CancellationToken cancellationToken);

        Task<bool> UpdateOrganisation(Organisation organisation, CancellationToken cancellationToken);

        Task<IEnumerable<OrganisationCollection>> GetProviderAssignments(long ukprn, string collectionType = null);

        Task<IEnumerable<OrganisationCollection>> GetAllProviderAssignmentsAsync(IEnumerable<long> listOfProviderUkprns);

        Task<bool> UpdateAssignments(long ukprn, IEnumerable<OrganisationCollection> organisationCollections, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> DeleteAssignments(long ukprn, IEnumerable<OrganisationCollection> organisationCollections, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<Provider>> SearchProvidersInPimsAsync(string searchTerm, int count, bool includeActiveOnly = true);

        Task<IList<long>> FilterProvidersInJobMgmtAsync(IList<long> Ukprns, CancellationToken cancellationToken);

        Task<ProviderDetail> GetProviderAsync(CancellationToken cancellationToken, long ukprn, bool onlyActive = true);

        Task<List<long>> GetProvidersWithFundingClaims(List<long> activeProviders);

        Task<ProviderAddress> GetProviderAddressAsync(long ukprn);

        Task<List<long>> GetProvidersWithCollectionAssignmentAsync(List<long> ukprns = null);

        Task<List<Provider>> GetAllValidPimsProviders(IEnumerable<long> providerUkprns, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<Provider>> GetAllValidAndActivePimsProviders(IEnumerable<long> providerUkrpns, CancellationToken cancellationToken = default(CancellationToken));

        Task AddBulkOrganisationsAsync(IEnumerable<Organisation> organisations, CancellationToken cancellationToken = default(CancellationToken));

        Task AddBulkOrganisationCollectionsAsync(IEnumerable<OrganisationCollection> organisationCollections, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<OrganisationCollection>> GetOrgCollectionsByTypeAsync(IEnumerable<long> ukprns, string collectionType, CancellationToken cancellationToken);

        Task<IList<Provider>> GetProviderStatusInJobMgmtAsync(List<UkprnAndActive> pimsUkprnAndActives, CancellationToken cancellationToken);
    }
}
