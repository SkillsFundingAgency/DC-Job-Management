namespace ESFA.DC.Jobs.Model.Processing.Detail
{
    public class JobDetails
    {
        public long JobId { get; set; }

        public string FileName { get; set; }

        public long Ukprn { get; set; }

        public string ProviderName { get; set; }

        public double ProcessingTimeMilliSeconds { get; set; }

        public string CollectionName { get; set; }

        public int CollectionYear { get; set; }
    }
}
