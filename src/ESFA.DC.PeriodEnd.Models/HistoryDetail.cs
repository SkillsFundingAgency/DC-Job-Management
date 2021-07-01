using System;

namespace ESFA.DC.PeriodEnd.Models
{
    public class HistoryDetail
    {
        public int Period { get; set; }

        public int PeriodEndId { get; set; }

        public int Year { get; set; }

        public DateTime? PeriodEndStart { get; set; }

        public DateTime? PeriodEndFinish { get; set; }

        public TimeSpan? PeriodEndRuntimeDays { get; set; }

        public TimeSpan? PeriodEndRuntimeBusinessDays { get; set; }
    }
}