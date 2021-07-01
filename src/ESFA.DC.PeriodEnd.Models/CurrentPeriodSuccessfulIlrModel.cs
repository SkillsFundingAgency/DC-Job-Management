namespace ESFA.DC.PeriodEnd.Models
{
    public sealed class CurrentPeriodSuccessfulIlrModel
    {
        public long Ukprn { get; set; }

        public long JobId { get; set; }

        public int CollectionYear { get; set; }

        public int PeriodNumber { get; set; }
    }
}
