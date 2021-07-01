using System;
using System.Text;

namespace ESFA.DC.Jobs.Model.Processing
{
    public abstract class ProcessingLookupModelBase
    {
        public long JobId { get; set; }

        public long Ukprn { get; set; }

        public string ProviderName { get; set; }

        public string CollectionType { get; set; }

        public virtual string GetDuration(int second)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(second);

            StringBuilder sb = new StringBuilder();

            if (timeSpan.Days > 0)
            {
                Append(sb, timeSpan.Days, "day");
            }

            if (timeSpan.Hours > 0)
            {
                Append(sb, timeSpan.Hours, "hour");
            }

            if (timeSpan.Minutes > 0)
            {
                Append(sb, timeSpan.Minutes, "minute");
            }

            if (timeSpan.Seconds > 0)
            {
                Append(sb, timeSpan.Seconds, "second");
            }

            return sb.ToString();
        }

        private void Append(StringBuilder sb, int time, string timeType)
        {
            sb.Append($"{(sb.Length > 0 ? " " : string.Empty)}{time} {timeType}{(time > 1 ? "s" : string.Empty)}");
        }
    }
}
