using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class SubPathValidityPeriod
    {
        public int HubPathId { get; set; }
        public int Period { get; set; }
        public int CollectionYear { get; set; }
        public bool? Enabled { get; set; }
    }
}
