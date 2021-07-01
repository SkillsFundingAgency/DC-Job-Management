using System;

namespace ESFA.DC.DashBoard.Models.Job
{
    public sealed class TodayStatsForYearModel
    {
        public int CollectionYear { get; set; }

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

        public int JobsProcessing { get; set; }

        public int JobsQueued { get; set; }

        public int FailedToday { get; set; }

        public int SubmissionsToday { get; set; }

        public int SubmissionsLastHour { get; set; }

        public int SubmissionsLast5Minutes { get; set; }

        private string GetFormattedTime(int time)
        {
            TimeSpan t = TimeSpan.FromSeconds(time);
            var extraMins = t.Hours * 60;
            return $"{(t.Minutes + extraMins):D2}m {t.Seconds:D2}s";
        }
    }
}