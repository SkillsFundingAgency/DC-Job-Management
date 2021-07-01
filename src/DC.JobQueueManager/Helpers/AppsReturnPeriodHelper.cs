using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager.Helpers
{
    public class AppsReturnPeriodHelper : IAppsReturnPeriodHelper
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly DateTime _firstAppsPeriodStartDate = new DateTime(2017, 5, 6);

        public AppsReturnPeriodHelper(
            Func<IJobQueueDataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<int> GetReturnPeriod(int collectionYear, int periodNumber)
        {
            var collectionName = "ILR" + collectionYear;
            ReturnPeriod returnPeriod;
            using (var context = _contextFactory())
            {
                returnPeriod = await context.ReturnPeriod
                    .Include(rp => rp.Collection)
                    .Where(rp => rp.Collection.Name == collectionName
                                       && rp.Collection.CollectionYear == collectionYear
                                       && rp.PeriodNumber == periodNumber)
                    .SingleAsync();
            }

            var calendarDate = new DateTime(returnPeriod.CalendarYear, returnPeriod.CalendarMonth, 1);

            var period = (((calendarDate.Year - _firstAppsPeriodStartDate.Year) * 12) + calendarDate.Month - _firstAppsPeriodStartDate.Month) + 1;

            return period;
        }
    }
}