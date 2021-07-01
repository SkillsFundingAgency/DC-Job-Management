using System;

namespace ESFA.DC.Jobs.Model
{
    [Serializable]
    public class FileUploadJob : Job
    {
        public string StorageReference { get; set; }

        public string FileName { get; set; }

        public decimal FileSize { get; set; }

        public int PeriodNumber { get; set; }

        public bool? IsFirstStage { get; set; }

        public DateTime? DateTimeSubmittedUtc { get; set; }

        public bool? TermsAccepted { get; set; }

        public int CollectionYear { get; set; }

        public string ContractReferenceNumber { get; set; }

        public string ProviderName { get; set; }

        public int? FisVersionNumber { get; set; }
    }
}
