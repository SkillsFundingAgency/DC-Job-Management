namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ServicePageMessage
    {
        public int PageId { get; set; }
        public int MessageId { get; set; }

        public virtual ServiceMessage Message { get; set; }
        public virtual ServicePage Page { get; set; }
    }
}
