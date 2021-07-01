using System;

namespace ESFA.DC.JobQueueManager.Data.ReadOnlyEntities
{
    public class ReadOnlyJobFailedToday : ReadOnlyJobBase
    {
        public DateTime FailedAt { get; set; }

        public int ProcessingTimeBeforeFailureSecond { get; set; }

        public string FileName { get; set; }
    }
}