namespace ESFA.DC.Jobs.Model
{
    public class ReportsPublicationJobMetaData
    {
        public long JobId { get; set; }

        public string SourceFolderKey { get; set; }

        public string SourceContainerName { get; set; }

        public int PeriodNumber { get; set; }

        public string StorageReference { get; set; }
    }
}
