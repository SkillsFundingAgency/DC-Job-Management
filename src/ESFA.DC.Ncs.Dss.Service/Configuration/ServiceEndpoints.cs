using Newtonsoft.Json;

namespace ESFA.DC.Ncs.Dss.Service.Configuration
{
    public class ServiceEndpoints
    {
        [JsonRequired]
        public string JobApiEndPoint { get; set; }
    }
}
