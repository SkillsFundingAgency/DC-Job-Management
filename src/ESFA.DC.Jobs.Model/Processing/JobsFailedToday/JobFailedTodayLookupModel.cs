using System;

namespace ESFA.DC.Jobs.Model.Processing.JobsFailedToday
{
    public class JobFailedTodayLookupModel : ProcessingLookupModelBase
    {
        public DateTime FailedAt { get; set; }

        public int ProcessingTimeBeforeFailureSecond { get; set; }

        public string ProcessingTimeBeforeFailure
        {
            get
            {
                return GetDuration(ProcessingTimeBeforeFailureSecond);
            }
        }

        public string FileName { get; set; }
    }
}