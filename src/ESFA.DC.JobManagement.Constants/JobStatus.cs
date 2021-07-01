namespace ESFA.DC.JobManagement.Constants
{
    public class JobStatus
    {
        public const int Ready = 1;
        public const int MovedForProcessing = 2;
        public const int Processing = 3;
        public const int Completed = 4;
        public const int FailedRetry = 5;
        public const int Failed = 6;
        public const int Paused = 7;
        public const int Waiting = 8;
    }
}
