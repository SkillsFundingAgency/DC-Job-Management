using System;

namespace ESFA.DC.Jobs.Model.Processing.JobsConcern
{
    public class JobConcernLookupModel : ProcessingLookupModelBase
    {
        public string FileName { get; set; }

        public DateTime LastSuccessfulSubmission { get; set; }

        public string PeriodOfLastSuccessfulSubmission { get; set; }
    }
}