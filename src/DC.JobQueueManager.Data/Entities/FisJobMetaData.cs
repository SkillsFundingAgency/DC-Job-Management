using System;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class FisJobMetaData
    {
        public int Id { get; set; }
        public long JobId { get; set; }
        public int VersionNumber { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public DateTime? PublishedDate { get; set; }
        public bool? IsRemoved { get; set; }

        public virtual Job Job { get; set; }
    }
}
