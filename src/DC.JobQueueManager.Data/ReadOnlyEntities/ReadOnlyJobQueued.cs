namespace ESFA.DC.JobQueueManager.Data.ReadOnlyEntities
{
    public class ReadOnlyJobQueued : ReadOnlyJobBase
    {
        public int TimeInQueueSecond { get; set; }

        public string CollectionType { get; set; }

        public int Status { get; set; }

        public string StatusDescription { get; set; }
    }
}