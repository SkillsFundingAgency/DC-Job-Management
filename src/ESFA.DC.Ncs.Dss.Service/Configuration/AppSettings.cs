using Newtonsoft.Json;

namespace ESFA.DC.Ncs.Dss.Service.Configuration
{
    public class AppSettings
    {
        [JsonRequired]
        public string InstrumentationKey { get; set; }
    }
}
