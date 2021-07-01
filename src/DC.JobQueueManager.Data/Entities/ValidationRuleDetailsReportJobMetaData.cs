namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ValidationRuleDetailsReportJobMetaData
    {
        public int Id { get; set; }
        public string Rule { get; set; }
        public int SelectedCollectionYear { get; set; }
        public long JobId { get; set; }

        public virtual Job Job { get; set; }
    }
}
