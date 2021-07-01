namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ApiAvailability
    {
        public string ApiName { get; set; }
        public string Process { get; set; }
        public bool? Enabled { get; set; }
    }
}
