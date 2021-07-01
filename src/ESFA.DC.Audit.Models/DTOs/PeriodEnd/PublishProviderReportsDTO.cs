namespace ESFA.DC.Audit.Models.DTOs.PeriodEnd
{
    public class PublishProviderReportsDTO
    {
        public int? CollectionYear { get; set; }

        public int Period { get; set; }

        public string CollectionType { get; set; }

        public bool IsPublished { get; set; }
    }
}
