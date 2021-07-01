namespace ESFA.DC.Jobs.Model.Processing.JobsSlowFile
{
    public class JobSlowFileLookupModel : ProcessingLookupModelBase
    {
        public string FileName { get; set; }

        public int TimeTakenSecond { get; set; }

        public int AverageTimeSecond { get; set; }

        public string TimeTaken
        {
            get
            {
                return GetDuration(TimeTakenSecond);
            }
        }

        public string AverageTime
        {
            get
            {
                return GetDuration(AverageTimeSecond);
            }
        }
    }
}