namespace ESFA.DC.Jobs.Model
{
    using System;

    [Serializable]
    public class ReportsPublicationJob : Job
    {
        public string SourceFolderKey { get; set; }

        public string SourceContainerName { get; set; }

        public int PeriodNumber { get; set; }

        public string StorageReference { get; set; }
    }
}
