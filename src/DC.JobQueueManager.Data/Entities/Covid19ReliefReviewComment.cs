using System;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class Covid19ReliefReviewComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateTimeAddedUtc { get; set; }
        public int Covid19ReliefSubmissionId { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? ApprovedDateTimeUtc { get; set; }
    }
}
