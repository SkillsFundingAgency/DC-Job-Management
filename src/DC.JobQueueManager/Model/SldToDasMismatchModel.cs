using System;

namespace ESFA.DC.JobQueueManager.Model
{
    public class SldToDasMismatchModel
    {
        public long Ukprn { get; set; }

        public long JobId { get; set; }

        public string Filename { get; set; }

        public DateTime DateTimeSubmittedUtc { get; set; }
    }
}
