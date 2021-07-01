namespace ESFA.DC.PeriodEnd.Models
{
    public class SubPathValidityPeriodLookupModel
    {
        public int PathId { get; set; }

        public int CollectionYear { get; set; }

        public int Period { get; set; }

        public bool? Enabled { get; set; }
    }
}