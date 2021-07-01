namespace ESFA.DC.Audit.Models.DTOs.Providers 
{
    public class ProviderManipulationDTO
    { 
        public string Name { get; set; }

        public long UKPRN { get; set; }

        public int UPIN { get; set; }

        public bool IsMCA { get; set; }
    }
}