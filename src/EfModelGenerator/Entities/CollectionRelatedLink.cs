using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class CollectionRelatedLink
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public int SortOrder { get; set; }

        public virtual Collection Collection { get; set; }
    }
}
