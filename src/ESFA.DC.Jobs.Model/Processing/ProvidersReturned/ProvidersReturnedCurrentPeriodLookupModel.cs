using System;

namespace ESFA.DC.Jobs.Model.Processing.ProvidersReturned
{
    public class ProvidersReturnedCurrentPeriodLookupModel : ProcessingLookupModelBase
    {
        public string FileName { get; set; }

        public DateTime DateTimeSubmission { get; set; }

        public long ProcessingTime { get; set; }
    }
}
