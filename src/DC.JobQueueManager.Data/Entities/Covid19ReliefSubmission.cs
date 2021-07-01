using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class Covid19ReliefSubmission
    {
        public Covid19ReliefSubmission()
        {
            Covid19ReliefQuestion = new HashSet<Covid19ReliefQuestion>();
        }

        public int Covid19ReliefSubmissionId { get; set; }
        public string FileName { get; set; }
        public DateTime DateTimeSubmittedUtc { get; set; }
        public string SubmittedBy { get; set; }
        public int CollectionId { get; set; }
        public int ReturnPeriodId { get; set; }
        public long Ukprn { get; set; }
        public string ProviderName { get; set; }
        public string Address { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual ReturnPeriod ReturnPeriod { get; set; }
        public virtual ICollection<Covid19ReliefQuestion> Covid19ReliefQuestion { get; set; }
    }
}
