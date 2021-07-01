namespace ESFA.DC.JobQueueManager.Data.ReadOnlyEntities
{
    public class ReadOnlyJobProcessingDetail : ReadOnlyJobBase
    {
        public string FileName { get; set; }

        public double ProcessingTimeSeconds { get; set; }
    }
}