using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class Recipient
    {
        public Recipient()
        {
            RecipientGroupRecipient = new HashSet<RecipientGroupRecipient>();
        }

        public int RecipientId { get; set; }
        public string EmailAddress { get; set; }

        public virtual ICollection<RecipientGroupRecipient> RecipientGroupRecipient { get; set; }
    }
}
