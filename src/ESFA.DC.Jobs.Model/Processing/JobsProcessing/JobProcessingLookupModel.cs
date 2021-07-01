namespace ESFA.DC.Jobs.Model.Processing.JobsProcessing
{
    public class JobProcessingLookupModel : ProcessingLookupModelBase
    {
        public int TimeTakenSecond { get; set; }

        public int DateDifferSecond { get; set; }

        public string CollectionType { get; set; }

        public int Status { get; set; }

        public string StatusDescription { get; set; }

        public string TimeTaken
        {
            get
            {
                return GetDuration(TimeTakenSecond);
            }
        }

        public string AverageProcessingTime
        {
            get
            {
                return GetDuration(DateDifferSecond);
            }
        }
    }
}
