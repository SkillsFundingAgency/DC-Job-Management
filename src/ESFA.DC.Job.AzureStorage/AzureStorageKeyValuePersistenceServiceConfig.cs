using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using Newtonsoft.Json;

namespace ESFA.DC.Job.AzureStorage
{
    public class AzureStorageKeyValuePersistenceServiceConfig : IAzureStorageKeyValuePersistenceServiceConfig
    {
        [JsonRequired]
        public string ConnectionString { get; set; }

        [JsonRequired]
        public string ContainerName { get; set; }
    }
}
