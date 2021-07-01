using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Services
{
    public class PeriodEndDateTimeService : IPeriodEndDateTimeService
    {
        public TimeSpan CalculateRuntimeSimple(DateTime? startDate, DateTime? endDate)
        {
            var periodFinished = startDate != null && endDate != null;

            return periodFinished ? endDate.Value - startDate.Value : new TimeSpan();
        }

        public async Task<PeriodEndWeekendModel> CalculateRuntimeBusiness(DateTime? startDate, DateTime? endDate)
        {
            if (startDate != null && endDate != null)
            {
                int weekendDays = 0;
                List<DateTime> weekendList = new List<DateTime>();
                var tempStartDate = startDate;
                while (tempStartDate <= endDate)
                {
                    if ((tempStartDate.Value.DayOfWeek == DayOfWeek.Saturday || tempStartDate.Value.DayOfWeek == DayOfWeek.Sunday) && tempStartDate != startDate)
                    {
                        weekendDays++;
                        weekendList.Add(tempStartDate.Value.Date);
                    }

                    tempStartDate = tempStartDate.Value.AddDays(1);
                }

                if (startDate.Value.DayOfWeek == DayOfWeek.Saturday || startDate.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = startDate.Value.Date;
                }

                if (endDate.Value.DayOfWeek == DayOfWeek.Saturday || endDate.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    endDate = endDate.Value.Date;
                }

                var weekendDaysTimeSpan = new TimeSpan(weekendDays, 0, 0, 0);
                return new PeriodEndWeekendModel()
                {
                    TimeDifference = (endDate.Value - startDate.Value).Add(-weekendDaysTimeSpan),
                    WeekendDays = weekendList
                };
            }

            return new PeriodEndWeekendModel()
            {
                TimeDifference = TimeSpan.Zero,
                WeekendDays = new List<DateTime>(),
            };
        }
    }
}
