using System;

namespace ESFA.DC.Ncs.Dss.Service.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToApiFormat(this DateTime dateToParse)
        {
            return
                $"{dateToParse.Year:0000}-{dateToParse.Month:00}-{dateToParse.Day:00}T{dateToParse.Hour:00}:{dateToParse.Minute:00}:{dateToParse.Second:00}.{dateToParse.Millisecond:000}Z";
        }
    }
}
