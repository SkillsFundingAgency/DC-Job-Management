namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class EmailValidityPeriod
    {
        public int HubEmailId { get; set; }
        public int HubPathItemId { get; set; }
        public int CollectionYear { get; set; }
        public int Period { get; set; }
        public bool? Enabled { get; set; }
    }
}
