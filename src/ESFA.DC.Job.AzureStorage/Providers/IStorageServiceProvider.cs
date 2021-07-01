using System.Threading.Tasks;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.Job.AzureStorage.Providers
{
    public interface IStorageServiceProvider
    {
        Task<IStreamableKeyValuePersistenceService> GetAzureStorageReferenceService(long ukprn, string collectionName);

        Task<AzureStorageKeyValuePersistenceServiceConfig> GetAzureStorageConfiguration(long ukprn, string collectionName);
    }
}
