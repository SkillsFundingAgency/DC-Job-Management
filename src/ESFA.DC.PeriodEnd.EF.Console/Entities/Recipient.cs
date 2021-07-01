using System;
using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF.Console.Entities
{
    public partial class Recipient
    {
        public Recipient()
        {
            RecipientGroupRecipients = new HashSet<RecipientGroupRecipient>();
        }

        public int RecipientId { get; set; }
        public string EmailAddress { get; set; }

        public virtual ICollection<RecipientGroupRecipient> RecipientGroupRecipients { get; set; }
    }
}
