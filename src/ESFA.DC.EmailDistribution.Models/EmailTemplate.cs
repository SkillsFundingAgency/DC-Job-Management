namespace ESFA.DC.EmailDistribution.Models
{
    public class EmailTemplate
    {
        public int EmailId { get; set; }

        public int HubEmailId { get; set; }

        public string TemplateId { get; set; }

        public string TemplateName { get; set; }

        public string TriggerPointName { get; set; }

        public RecipientGroup RecipientGroup { get; set; }
    }
}
