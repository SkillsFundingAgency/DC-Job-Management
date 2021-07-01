namespace ESFA.DC.PeriodEnd.Models
{
    public class ValidityPeriodLookupModel
    {
        public int PathItemId { get; set; }

        public int CollectionYear { get; set; }

        public int Period { get; set; }

        public bool? Enabled { get; set; }
    }
}