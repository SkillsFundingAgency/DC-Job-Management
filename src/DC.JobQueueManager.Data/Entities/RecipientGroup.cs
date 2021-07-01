using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class RecipientGroup
    {
        public RecipientGroup()
        {
            EmailRecipientGroup = new HashSet<EmailRecipientGroup>();
            RecipientGroupRecipient = new HashSet<RecipientGroupRecipient>();
        }

        public int RecipientGroupId { get; set; }
        public string GroupName { get; set; }

        public virtual ICollection<EmailRecipientGroup> EmailRecipientGroup { get; set; }
        public virtual ICollection<RecipientGroupRecipient> RecipientGroupRecipient { get; set; }
    }
}
