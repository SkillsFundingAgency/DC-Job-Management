namespace ESFA.DC.Jobs.Model.Processing.JobsProcessing
{
    public class JobProcessingDetailLookupModel : ProcessingLookupModelBase
    {
        public string FileName { get; set; }

        public double ProcessingTimeSeconds { get; set; }
    }
}
