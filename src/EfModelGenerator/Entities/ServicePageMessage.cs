using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ServicePageMessage
    {
        public int PageId { get; set; }
        public int MessageId { get; set; }

        public virtual ServiceMessage1 Message { get; set; }
        public virtual ServicePage Page { get; set; }
    }
}
