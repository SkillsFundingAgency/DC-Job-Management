using System;
using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF.Console.Entities
{
    public partial class Job
    {
        public Job()
        {
            PathItemJobs = new HashSet<PathItemJob>();
        }

        public long JobId { get; set; }
        public int CollectionId { get; set; }
        public short Priority { get; set; }
        public DateTime DateTimeCreatedUtc { get; set; }
        public DateTime? DateTimeUpdatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public short Status { get; set; }
        public byte[] RowVersion { get; set; }
        public string NotifyEmail { get; set; }
        public short? CrossLoadingStatus { get; set; }
        public long? Ukprn { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual ICollection<PathItemJob> PathItemJobs { get; set; }
    }
}
