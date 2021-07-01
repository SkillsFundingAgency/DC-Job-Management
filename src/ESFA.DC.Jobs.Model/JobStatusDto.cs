namespace ESFA.DC.Jobs.Model
{
    /// <summary>
    /// The Job Status DTO transported across queues and Web API, must be serialisable.
    /// </summary>
    public class JobStatusDto
    {
        public JobStatusDto()
        {
        }

        public JobStatusDto(long jobId, int jobStatus, int numberOfLearners = -1, bool continueToFailJob = true)
        {
            JobId = jobId;
            JobStatus = jobStatus;
            NumberOfLearners = numberOfLearners;
            ContinueToFailJob = continueToFailJob;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public long JobId { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public int JobStatus { get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        public int NumberOfLearners { get; set; }

        public bool ContinueToFailJob { get; set; } = true;
    }
}
