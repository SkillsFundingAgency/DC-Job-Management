using System;

namespace ESFA.DC.CovidRelief.Models
{
    public class SubmissionDate
    {
        public long Ukprn { get; set; }

        public DateTime DateTimeSubmittedUtc { get; set; }
    }
}
