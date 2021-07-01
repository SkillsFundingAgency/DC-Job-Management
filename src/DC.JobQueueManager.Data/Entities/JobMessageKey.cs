namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class JobMessageKey
    {
        public int Id { get; set; }
        public int? CollectionId { get; set; }
        public string MessageKey { get; set; }
        public bool? IsFirstStage { get; set; }
    }
}
