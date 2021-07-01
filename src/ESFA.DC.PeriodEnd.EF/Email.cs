using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF
{
    public partial class Email
    {
        public Email()
        {
            EmailRecipientGroups = new HashSet<EmailRecipientGroup>();
        }

        public int EmailId { get; set; }
        public int HubEmailId { get; set; }
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TriggerPointName { get; set; }

        public virtual ICollection<EmailRecipientGroup> EmailRecipientGroups { get; set; }
    }
}
