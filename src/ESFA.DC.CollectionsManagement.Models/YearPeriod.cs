using System;

namespace ESFA.DC.CollectionsManagement.Models
{
    public sealed class YearPeriod
    {
        public int Year { get; set; }

        public int Period { get; set; }

        public DateTime PeriodEndDate { get; set; }

        public bool PeriodClosed { get; set; }
    }
}
