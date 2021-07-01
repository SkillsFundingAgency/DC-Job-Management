namespace ESFA.DC.JobQueueManager.Data.ReadOnlyEntities
{
    public class ReadOnlyJobSlowFile : ReadOnlyJobBase
    {
        public string FileName { get; set; }

        public int TimeTakenSecond { get; set; }

        public int AverageTimeSecond { get; set; }
    }
}