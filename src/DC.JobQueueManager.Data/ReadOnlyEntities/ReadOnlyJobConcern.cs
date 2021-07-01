using System;

namespace ESFA.DC.JobQueueManager.Data.ReadOnlyEntities
{
    public class ReadOnlyJobConcern : ReadOnlyJobBase
    {
        public string FileName { get; set; }

        public DateTime LastSuccessfulSubmission { get; set; }

        public string PeriodOfLastSuccessfulSubmission { get; set; }
    }
}