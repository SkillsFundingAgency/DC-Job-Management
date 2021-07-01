namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class EsfMetaData
    {
        public int Id { get; set; }
        public long JobId { get; set; }
        public string ContractReferenceNumber { get; set; }
        public int Round { get; set; }

        public virtual Job Job { get; set; }
    }
}
