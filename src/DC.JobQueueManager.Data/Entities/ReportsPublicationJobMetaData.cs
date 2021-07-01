namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ReportsPublicationJobMetaData
    {
        public int Id { get; set; }
        public string SourceFolderKey { get; set; }
        public string SourceContainerName { get; set; }
        public long JobId { get; set; }
        public int PeriodNumber { get; set; }
        public string StorageReference { get; set; }
        public bool? ReportsPublished { get; set; }

        public virtual Job Job { get; set; }
    }
}
