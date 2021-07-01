using System;
using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF.Console.Entities
{
    public partial class PathItem
    {
        public PathItem()
        {
            PathItemJobs = new HashSet<PathItemJob>();
        }

        public int PathItemId { get; set; }
        public int PathId { get; set; }
        public int Ordinal { get; set; }
        public bool? IsPausing { get; set; }

        public virtual Path Path { get; set; }
        public virtual ICollection<PathItemJob> PathItemJobs { get; set; }
    }
}
