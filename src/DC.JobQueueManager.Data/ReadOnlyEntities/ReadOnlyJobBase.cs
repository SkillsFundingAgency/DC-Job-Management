namespace ESFA.DC.JobQueueManager.Data.ReadOnlyEntities
{
    public abstract class ReadOnlyJobBase
    {
        public long JobId { get; set; }

        public int CollectionYear { get; set; }

        public long Ukprn { get; set; }

        public string CollectionType { get; set; }
    }
}