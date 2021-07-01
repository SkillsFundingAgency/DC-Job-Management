using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Job.WebApi.Settings;
using ESFA.DC.JobQueueManager.Interfaces;

namespace ESFA.DC.Job.WebApi.Providers
{
    public class StorageServiceProvider : IStorageServiceProvider
    {
        private readonly ICollectionService _collectionService;
        private readonly CloudStorageSettings _cloudStorageSettings;

        public StorageServiceProvider(
            ICollectionService collectionService,
            CloudStorageSettings cloudStorageSettings)
        {
            _collectionService = collectionService;
            _cloudStorageSettings = cloudStorageSettings;
        }

        public async Task<IStreamableKeyValuePersistenceService> GetAzureStorageReferenceServiceAsync(CancellationToken cancellationToken, string collectionName)
        {
            var config = await GetAzureStorageConfigurationAsync(cancellationToken, collectionName);
            return new AzureStorageKeyValuePersistenceService(config);
        }

        public async Task<AzureStorageKeyValuePersistenceServiceConfig> GetAzureStorageConfigurationAsync(CancellationToken cancellationToken, string collectionName)
        {
            var collection = await _collectionService.GetCollectionFromNameAsync(cancellationToken, collectionName);

            var config = new AzureStorageKeyValuePersistenceServiceConfig
            {
                ConnectionString = _cloudStorageSettings.ConnectionStrings[$"{collection.CollectionType}ConnectionString"],
                ContainerName = collection.StorageReference
            };

            return config;
        }
    }
}
