namespace ESFA.DC.DashBoard.Models.Job
{
    public sealed class JobDetailsModel
    {
        public short JobStatusId { get; set; }

        public string JobStatus { get; set; }

        public int JobCount { get; set; }

        public string CollectionName { get; set; }
    }
}