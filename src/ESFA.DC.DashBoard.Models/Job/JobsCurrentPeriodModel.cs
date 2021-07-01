namespace ESFA.DC.DashBoard.Models.Job
{
    public sealed class JobsCurrentPeriodModel
    {
        public int JobsCurrentPeriod { get; set; }

        public int JobsFailedInPeriod { get; set; }

        public int CollectionYear { get; set; }

        public int PeriodNumber { get; set; }
    }
}