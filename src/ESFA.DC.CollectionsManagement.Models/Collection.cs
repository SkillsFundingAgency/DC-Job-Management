using System;

namespace ESFA.DC.CollectionsManagement.Models
{
    public class Collection
    {
        public string CollectionTitle { get; set; }

        public bool IsOpen { get; set; }

        public string CollectionType { get; set; }

        public int CollectionYear { get; set; }

        public string Description { get; set; }

        public string SubText { get; set; }

        public int CollectionId { get; set; }

        public bool EmailOnJobCreation { get; set; }

        public string StorageReference { get; set; }

        public int? OpenPeriodNumber { get; set; }

        public string FileNameRegex { get; set; }

        public int? LastPeriodNumber { get; set; }

        public DateTime? LastPeriodClosedDate { get; set; }

        public DateTime? OpenPeriodCloseDate { get; set; }

        public DateTime? PeriodCloseDate { get; set; }

        public string FundingClaimPeriodNumber { get; set; }

        public bool? ProcessingOverride { get; set; }

        public int? NextPeriodNumber { get; set; }

        public DateTime? NextPeriodOpenDateTimeUtc { get; set; }

        public DateTime? StartDateTimeUtc { get; set; }

        public DateTime? EndDateTimeUtc { get; set; }

        public bool IsProviderAssignableInOperations { get; set; }

        public bool IsManageableInOperations { get; set; }
    }
}
