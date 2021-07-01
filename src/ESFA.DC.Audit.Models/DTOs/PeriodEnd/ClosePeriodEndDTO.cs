namespace ESFA.DC.Audit.Models.DTOs.PeriodEnd
{
    public class ClosePeriodEndDTO
    {
        public int? CollectionYear { get; set; }

        public int Period { get; set; }

        public string CollectionType { get; set; }

        public bool IsClosed { get; set; }
    }
}
