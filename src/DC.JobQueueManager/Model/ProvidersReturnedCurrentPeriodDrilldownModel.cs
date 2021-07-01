using System;

namespace ESFA.DC.JobQueueManager.Model
{
    public class ProvidersReturnedCurrentPeriodDrilldownModel
    {
        public long Ukprn { get; set; }

        public long JobId { get; set; }

        public string Filename { get; set; }

        public int CollectionYear { get; set; }

        public DateTime DateTimeSubmission { get; set; }

        public long ProcessingTime { get; set; }
    }
}
