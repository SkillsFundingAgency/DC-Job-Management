using System;
using ESFA.DC.JobQueueManager.Data.Entities;

namespace ESFA.DC.JobQueueManager.Data.Extensions
{
    public static class ReturnPeriodExtensions
    {
        public static bool IsCurrentPeriod(this ReturnPeriod returnPeriod, DateTime now)
        {
            return returnPeriod.StartDateTimeUtc <= now && returnPeriod.EndDateTimeUtc >= now;
        }

        public static bool IsEarlierPeriod(this ReturnPeriod returnPeriod, DateTime now)
        {
            return returnPeriod.EndDateTimeUtc <= now;
        }
    }
}
