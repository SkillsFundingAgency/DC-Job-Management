using System;

namespace ESFA.DC.JobQueueManager.Model
{
    public class JobsFailedCurrentPeriodDrilldownModel
    {
        public long JobId { get; set; }

        public int CollectionYear { get; set; }

        public string CollectionType { get; set; }

        public long Ukprn { get; set; }

        public string Filename { get; set; }

        public DateTime DateTimeOfFailure { get; set; }

        public DateTime ProcessingTimeBeforeFailure { get; set; }
    }
}
