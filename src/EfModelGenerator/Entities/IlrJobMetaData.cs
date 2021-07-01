using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class IlrJobMetaData
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public DateTime DateTimeSubmittedUtc { get; set; }

        public virtual Job Job { get; set; }
    }
}
