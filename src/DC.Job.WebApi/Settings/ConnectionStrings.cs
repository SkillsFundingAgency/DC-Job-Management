using Newtonsoft.Json;

namespace ESFA.DC.Job.WebApi.Settings
{
    public class ConnectionStrings
    {
        [JsonRequired]
        public string AppLogs { get; set; }

        [JsonRequired]
        public string FCSReferenceData { get; set; }

        [JsonRequired]
        public string ORGReferenceData { get; set; }

        [JsonRequired]
        public string SummarisedActualsData { get; set; }

        [JsonRequired]
        public string ILR1819DataStore { get; set; }

        [JsonRequired]
        public string ILR1920DataStore { get; set; }

        [JsonRequired]
        public string ILR2021DataStore { get; set; }

        [JsonRequired]
        public string ILR2122DataStore { get; set; }

        [JsonRequired]
        public string ValidationMessages { get; set; }

        [JsonRequired]
        public string PIMSData { get; set; }

        [JsonRequired]
        public string DasPayments { get; set; }

        [JsonRequired]
        public string OpsAudit { get; set; }
    }
}
