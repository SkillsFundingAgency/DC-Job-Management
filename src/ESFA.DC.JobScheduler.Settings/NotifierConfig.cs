using ESFA.DC.JobNotifications;
using Newtonsoft.Json;

namespace ESFA.DC.JobScheduler.Settings
{
    public class NotifierConfig : INotifierConfig
    {
        [JsonRequired]
        public string ApiKey { get; set; }
    }
}
