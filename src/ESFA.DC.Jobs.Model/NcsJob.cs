using System;

namespace ESFA.DC.Jobs.Model
{
    [Serializable]
    public class NcsJob : Job
    {
        // NCS Meta Data
        public string ExternalJobId { get; set; }

        public string TouchpointId { get; set; }

        public DateTime ExternalTimestamp { get; set; }

        public string ReportFileName { get; set; }

        public string DssContainer { get; set; }

        public DateTime ReportEndDate { get; set; }

        public int Period { get; set; }
    }
}
