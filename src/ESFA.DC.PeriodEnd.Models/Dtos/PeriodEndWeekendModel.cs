using System;
using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.Models.Dtos
{
    public class PeriodEndWeekendModel
    {
        public TimeSpan TimeDifference { get; set; }

        public List<DateTime> WeekendDays { get; set; }
    }
}
