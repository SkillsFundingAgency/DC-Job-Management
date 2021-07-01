namespace ESFA.DC.PeriodEnd.EF
{
    public partial class ValidityPeriod
    {
        public int CollectionId { get; set; }
        public int Period { get; set; }
        public bool? Enabled { get; set; }

        public virtual Collection Collection { get; set; }
    }
}
