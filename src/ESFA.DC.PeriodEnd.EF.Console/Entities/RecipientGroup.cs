using System;
using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF.Console.Entities
{
    public partial class RecipientGroup
    {
        public RecipientGroup()
        {
            EmailRecipientGroups = new HashSet<EmailRecipientGroup>();
            RecipientGroupRecipients = new HashSet<RecipientGroupRecipient>();
        }

        public int RecipientGroupId { get; set; }
        public string GroupName { get; set; }

        public virtual ICollection<EmailRecipientGroup> EmailRecipientGroups { get; set; }
        public virtual ICollection<RecipientGroupRecipient> RecipientGroupRecipients { get; set; }
    }
}
