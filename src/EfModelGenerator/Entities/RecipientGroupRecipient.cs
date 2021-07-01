using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class RecipientGroupRecipient
    {
        public int RecipientGroupId { get; set; }
        public int RecipientId { get; set; }

        public virtual Recipient Recipient { get; set; }
        public virtual RecipientGroup RecipientGroup { get; set; }
    }
}
