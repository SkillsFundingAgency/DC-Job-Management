using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class PathItem
    {
        public PathItem()
        {
            PathItemJob = new HashSet<PathItemJob>();
        }

        public int PathItemId { get; set; }
        public int PathId { get; set; }
        public int Ordinal { get; set; }
        public bool? IsPausing { get; set; }
        public bool? HasJobs { get; set; }
        public bool Hidden { get; set; }
        public string PathItemLabel { get; set; }

        public virtual Path Path { get; set; }
        public virtual ICollection<PathItemJob> PathItemJob { get; set; }
    }
}
