using System;

namespace ESFA.DC.Jobs.Model
{
    public class JobMetaDataDto
    {
        public long JobId { get; set; }

        public int JobStatus { get; set; }

        public int PeriodNumber { get; set; }

        public string FileName { get; set; }

        public DateTime SubmissionDate { get; set; }

        public string SubmittedBy { get; set; }

        public int? VersionNumber { get; set; }

        public DateTime? EsfPublishedDate { get; set; }

        public DateTime? FisPublishedDate { get; set; }

        public bool? FisRemoveFlag { get; set; }
    }
}