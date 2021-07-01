using System;
using System.Collections.Generic;

namespace ESFA.DC.CovidRelief.Models
{
    public class Submission
    {
        public int SubmissionId { get; set; }

        public long Ukprn { get; set; }

        public string FileName { get; set; }

        public string SubmittedBy { get; set; }

        public DateTime SubmittedAt { get; set; }

        public string ProviderName { get; set; }

        public string ProviderAddress { get; set; }

        public int CollectionId { get; set; }

        public string CollectionName { get; set; }

        public int ReturnPeriodNumber { get; set; }

        public string ConfirmationEmail { get; set; }

        public List<SubmissionQuestion> Questions { get; set; }
    }
}
