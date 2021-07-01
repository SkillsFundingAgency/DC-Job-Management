namespace ESFA.DC.Jobs.Model.Processing.JobsQueued
{
    public class JobQueuedLookupModel : ProcessingLookupModelBase
    {
        public int TimeInQueueSecond { get; set; }

        public string CollectionType { get; set; }

        public int Status { get; set; }

        public string StatusDescription { get; set; }

        public string TimeInQueue
        {
            get
            {
                return GetDuration(TimeInQueueSecond);
            }
        }
    }
}
