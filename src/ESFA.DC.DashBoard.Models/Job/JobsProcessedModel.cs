namespace ESFA.DC.DashBoard.Models.Job
{
    public sealed class JobsProcessedModel
    {
        public int JobsProcessedToday { get; set; }

        public int JobsProcessedLastHour { get; set; }

        public int JobsProcessedLast5Minutes { get; set; }
    }
}