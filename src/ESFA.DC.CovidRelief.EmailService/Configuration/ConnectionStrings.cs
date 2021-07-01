using Newtonsoft.Json;

namespace ESFA.DC.CovidRelief.EmailService.Configuration
{
    public class ConnectionStrings
    {
        [JsonRequired]
        public string AppLogs { get; set; }

        [JsonRequired]
        public string JobManagement { get; set; }
    }
}
