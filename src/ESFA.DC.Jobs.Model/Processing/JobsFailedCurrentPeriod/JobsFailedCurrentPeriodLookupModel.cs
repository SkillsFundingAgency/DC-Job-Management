using System;

namespace ESFA.DC.Jobs.Model.Processing.JobsFailedCurrentPeriod
{
    public class JobsFailedCurrentPeriodLookupModel : ProcessingLookupModelBase
    {
        public string FileName { get; set; }

        public DateTime DateTimeOfFailure { get; set; }

        public DateTime ProcessingTimeBeforeFailure { get; set; }
    }
}
