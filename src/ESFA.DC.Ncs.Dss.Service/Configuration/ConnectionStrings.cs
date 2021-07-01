using Newtonsoft.Json;

namespace ESFA.DC.Ncs.Dss.Service.Configuration
{
    public class ConnectionStrings
    {
        [JsonRequired]
        public string LogConnectionString { get; set; }
    }
}
