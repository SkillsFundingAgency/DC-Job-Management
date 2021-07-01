using System;

namespace ESFA.DC.JobQueueManager.Data.ReadOnlyEntities
{
    public class ReadOnlyJob
    {
        public long JobId { get; set; }

        public short Status { get; set; }

        public DateTime DateTimeSubmittedUtc { get; set; }

        public string CreatedBy { get; set; }

        public long Ukprn { get; set; }

        public string CollectionName { get; set; }

        public string FileName { get; set; }

        public int PeriodNumber { get; set; }

        public int CalendarMonth { get; set; }

        public int CalendarYear { get; set; }

        public int CollectionYear { get; set; }

        public int CollectionId { get; set; }

        public string CollectionType { get; set; }

        public bool IsSubmitted { get; set; }

        public string NotifyEmail { get; set; }

        public string StorageReference { get; set; }

        public decimal FileSize { get; set; }

        public string ExternalJobId { get; set; }

        public string TouchpointId { get; set; }

        public DateTime? ExternalTimestamp { get; set; }

        public string ReportFileName { get; set; }

        public bool IsCollectionUploadType { get; set; }

        public string ContractReferenceNumber { get; set; }

        public string SourceContainerName { get; set; }

        public string SourceFolderKey { get; set; }

        public string DssContainer { get; set; }

        public DateTime ReportEndDate { get; set; }

        public string Rule { get; set; }

        public int SelectedCollectionYear { get; set; }

        public DateTime? DateTimeUpdatedUtc { get; set; }

        public DateTime DateTimeCreatedUtc { get; set; }
    }
}
