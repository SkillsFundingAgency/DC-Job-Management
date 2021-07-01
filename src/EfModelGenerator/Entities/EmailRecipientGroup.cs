using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class EmailRecipientGroup
    {
        public int EmailRecipientGroupId { get; set; }
        public int EmailId { get; set; }
        public int? RecipientGroupId { get; set; }

        public virtual Email Email { get; set; }
        public virtual RecipientGroup RecipientGroup { get; set; }
    }
}
