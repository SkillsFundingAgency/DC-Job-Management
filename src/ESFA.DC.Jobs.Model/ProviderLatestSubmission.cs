using System;

namespace ESFA.DC.Jobs.Model
{
    public class ProviderLatestSubmission
    {
        public string CollectionName { get; set; }

        public long JobId { get; set; }

        public int CollectionYear { get; set; }

        public int PeriodNumber { get; set; }

        public string LastSubmittedBy { get; set; }

        public DateTime LastSubmittedDateUtc { get; set; }

        public long Ukprn { get; set; }
    }
}
