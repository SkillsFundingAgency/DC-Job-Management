using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Job.WebApi.Settings;

namespace ESFA.DC.Job.WebApi.Providers
{
    public interface IStorageServiceProvider
    {
        Task<IStreamableKeyValuePersistenceService> GetAzureStorageReferenceServiceAsync(CancellationToken cancellationToken, string collectionName);

        Task<AzureStorageKeyValuePersistenceServiceConfig> GetAzureStorageConfigurationAsync(CancellationToken cancellationToken, string collectionName);
    }
}
