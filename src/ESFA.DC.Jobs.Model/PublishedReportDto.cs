namespace ESFA.DC.Jobs.Model
{
    public class PublishedReportDto
    {
        public int PeriodNumber { get; set; }

        public int? CollectionYear { get; set; }

        public int CollectionId { get; set; }

        public string CollectionName { get; set; }
    }
}
