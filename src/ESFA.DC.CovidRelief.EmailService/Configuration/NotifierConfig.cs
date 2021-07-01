using ESFA.DC.JobNotifications;
using Newtonsoft.Json;

namespace ESFA.DC.CovidRelief.EmailService.Configuration
{
    public class NotifierConfig : INotifierConfig
    {
        [JsonRequired]
        public string ApiKey { get; set; }
    }
}
