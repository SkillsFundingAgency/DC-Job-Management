namespace ESFA.DC.PeriodEnd.Models.Dtos
{
    public class PathItemJobSummaryModel
    {
        public int NumberOfWaitingJobs { get; set; }

        public int NumberOfRunningJobs { get; set; }

        public int NumberOfFailedJobs { get; set; }

        public int NumberOfCompleteJobs { get; set; }
    }
}
