using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class NcsJobMetaData
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public string ExternalJobId { get; set; }
        public string TouchpointId { get; set; }
        public DateTime ExternalTimestamp { get; set; }
        public string ReportFileName { get; set; }
        public string DssContainer { get; set; }
        public DateTime ReportEndDate { get; set; }
        public int PeriodNumber { get; set; }

        public virtual Job Job { get; set; }
    }
}
