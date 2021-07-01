namespace ESFA.DC.JobQueueManager.Data.ReadOnlyEntities
{
    public class ReadOnlyJobProcessing : ReadOnlyJobBase
    {
        public int TimeTakenSecond { get; set; }

        public int DateDifferSecond { get; set; }

        public string CollectionType { get; set; }

        public int Status { get; set; }

        public string StatusDescription { get; set; }
    }
}
