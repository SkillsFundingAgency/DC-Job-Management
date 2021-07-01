namespace ESFA.DC.Audit.Models.DTOs.PeriodEnd
{
    public class EditEmailTemplateDTO
    {
        public int EmailId { get; set;  }

        public string EmailTemplateName { get; set; }

        public int? GroupId { get; set; }
    }
}
