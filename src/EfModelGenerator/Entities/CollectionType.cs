using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class CollectionType
    {
        public CollectionType()
        {
            Collection = new HashSet<Collection>();
            ReportsArchive = new HashSet<ReportsArchive>();
        }

        public int CollectionTypeId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int ConcurrentExecutionCount { get; set; }
        public bool IsProviderAssignableInOperations { get; set; }
        public bool IsManageableInOperations { get; set; }

        public virtual ICollection<Collection> Collection { get; set; }
        public virtual ICollection<ReportsArchive> ReportsArchive { get; set; }
    }
}
