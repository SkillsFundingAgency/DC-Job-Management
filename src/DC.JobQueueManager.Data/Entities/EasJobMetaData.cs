namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class EasJobMetaData
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public bool TermsAccepted { get; set; }

        public virtual Job Job { get; set; }
    }
}
