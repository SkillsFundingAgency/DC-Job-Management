namespace ESFA.DC.PeriodEnd.EF
{
    public partial class EmailValidityPeriod
    {
        public int HubEmailId { get; set; }
        public int Period { get; set; }
        public bool? Enabled { get; set; }
    }
}
