using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class EsfJobMetaData
    {
        public int Id { get; set; }
        public long JobId { get; set; }
        public string ContractReferenceNumber { get; set; }
        public DateTime? PublishedDate { get; set; }

        public virtual Job Job { get; set; }
    }
}
