using System;

namespace ESFA.DC.Data.Models
{
    public class IlrFileDetails
    {
        public long Ukprn { get; set; }

        public string Filename { get; set; }

        public decimal FileSizeKb { get; set; }

        public int TotalLearnersSubmitted { get; set; }

        public int TotalValidLearnersSubmitted { get; set; }

        public int TotalInvalidLearnersSubmitted { get; set; }

        public int TotalErrorCount { get; set; }

        public int TotalWarningCount { get; set; }

        public DateTime SubmittedTime { get; set; }
    }
}
