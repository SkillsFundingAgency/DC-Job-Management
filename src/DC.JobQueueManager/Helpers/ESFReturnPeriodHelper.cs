using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager.Helpers
{
    public class ESFReturnPeriodHelper : IESFReturnPeriodHelper
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly DateTime FirstESFPeriodDate = new DateTime(2016, 1, 1);

        public ESFReturnPeriodHelper(
            Func<IJobQueueDataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<int> GetESFReturnPeriod(int collectionYear, int periodNumber)
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

            var period = (((calendarDate.Year - FirstESFPeriodDate.Year) * 12) + calendarDate.Month - FirstESFPeriodDate.Month) + 1;

            return period;
        }
    }
}