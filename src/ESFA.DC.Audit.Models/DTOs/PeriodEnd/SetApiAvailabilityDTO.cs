namespace ESFA.DC.Audit.Models.DTOs.PeriodEnd
{
    public class SetApiAvailabilityDTO
    {
        public string ApiName { get; set; }

        public string Process { get; set; }

        public bool? Enabled { get; set; }
    }
}
