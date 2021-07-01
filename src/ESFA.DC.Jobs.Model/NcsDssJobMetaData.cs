using System;

namespace ESFA.DC.Jobs.Model
{
    public class NcsDssJobMetaData
    {
        public string ExternalJobId { get; set; }

        public string TouchpointId { get; set; }

        public DateTime ExternalTimestamp { get; set; }

        public string DssContainer { get; set; }

        public string ReportFileName { get; set; }

        public DateTime ReportEndDate { get; set; }
    }
}
