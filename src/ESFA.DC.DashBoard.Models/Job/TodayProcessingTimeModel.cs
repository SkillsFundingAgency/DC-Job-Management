using System;

namespace ESFA.DC.DashBoard.Models.Job
{
    public class TodayProcessingTimeModel
    {
        public int AverageTimeToday { get; set; }

        public string AverageProcessingTime
        {
            get { return GetFormattedTime(AverageTimeToday); }
        }

        public int AverageTimeLastHour { get; set; }

        public string AverageProcessingTimeLastHour
        {
            get { return GetFormattedTime(AverageTimeLastHour); }
        }

        private string GetFormattedTime(int time)
        {
            TimeSpan t = TimeSpan.FromSeconds(time);
            var extraMins = t.Hours * 60;
            return $"{(t.Minutes + extraMins):D2}m {t.Seconds:D2}s";
        }
    }
}
