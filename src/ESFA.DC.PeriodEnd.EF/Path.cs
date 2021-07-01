using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF
{
    public partial class Path
    {
        public Path()
        {
            PathItems = new HashSet<PathItem>();
        }

        public int PathId { get; set; }
        public int PeriodEndId { get; set; }
        public int HubPathId { get; set; }

        public virtual PeriodEnd PeriodEnd { get; set; }
        public virtual ICollection<PathItem> PathItems { get; set; }
    }
}
