using System;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.Jobs.Model
{
    [Serializable]

    public class Job
    {
        public long JobId { get; set; }

        public JobStatusType Status { get; set; }

        public short Priority { get; set; }

        public DateTime DateTimeCreatedUtc { get; set; }

        public DateTime? DateTimeUpdatedUtc { get; set; }

        public string RowVersion { get; set; }

        public string CreatedBy { get; set; }

        public string NotifyEmail { get; set; }

        public JobStatusType? CrossLoadingStatus { get; set; }

        public long Ukprn { get; set; }

        public string CollectionName { get; set; }
    }
}
