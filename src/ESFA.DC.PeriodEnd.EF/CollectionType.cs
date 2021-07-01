using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF
{
    public partial class CollectionType
    {
        public CollectionType()
        {
            Collections = new HashSet<Collection>();
        }

        public int CollectionTypeId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int ConcurrentExecutionCount { get; set; }

        public virtual ICollection<Collection> Collections { get; set; }
    }
}
