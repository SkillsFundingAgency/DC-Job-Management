using System;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ReturnPeriodFundingClaimsOverride
    {
        public int Id { get; set; }
        public int CollectionId { get; set; }
        public DateTime? SignatureCloseDateUtc { get; set; }
        public bool? RequiresSignature { get; set; }
        public string CollectionCode { get; set; }
        public int SummarisedPeriodFrom { get; set; }
        public int SummarisedPeriodTo { get; set; }
        public string SummarisedReturnPeriod { get; set; }
        public string Period { get; set; }
        public string PeriodTypeCode { get; set; }
        public string ClaimTypeName { get; set; }
    }
}
