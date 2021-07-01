using System.Threading.Tasks;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.Job.AzureStorage.Providers
{
    public class StorageServiceProvider : IStorageServiceProvider
    {
        private readonly ICollectionManagementService _collectionService;
        private readonly CloudStorageSettings _cloudStorageSettings;

        public StorageServiceProvider(
            ICollectionManagementService collectionService,
            CloudStorageSettings cloudStorageSettings)
        {
            _collectionService = collectionService;
            _cloudStorageSettings = cloudStorageSettings;
        }

        public async Task<IStreamableKeyValuePersistenceService> GetAzureStorageReferenceService(long ukprn, string collectionName)
        {
            var config = await GetAzureStorageConfiguration(ukprn, collectionName);
            return new AzureStorageKeyValuePersistenceService(config);
        }

        public async Task<AzureStorageKeyValuePersistenceServiceConfig> GetAzureStorageConfiguration(long ukprn, string collectionName)
        {
            var collection = await _collectionService.GetCollectionAsync(ukprn, collectionName);

            var config = new AzureStorageKeyValuePersistenceServiceConfig()
            {
                ConnectionString = _cloudStorageSettings.ConnectionStrings[$"{collection.CollectionType}ConnectionString"],
                ContainerName = collection.StorageReference,
            };

            return config;
        }
    }
}
