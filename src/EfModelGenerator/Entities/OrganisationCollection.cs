using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class OrganisationCollection
    {
        public int OrganisationId { get; set; }
        public int CollectionId { get; set; }
        public DateTime? StartDateTimeUtc { get; set; }
        public DateTime? EndDateTimeUtc { get; set; }
        public long? Ukprn { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual Organisation Organisation { get; set; }
    }
}
