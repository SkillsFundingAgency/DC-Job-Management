namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ValidityPeriod
    {
        public int HubPathItemId { get; set; }
        public int Period { get; set; }
        public int CollectionYear { get; set; }
        public bool? Enabled { get; set; }
    }
}
