namespace ESFA.DC.Audit.Models.DTOs.PeriodEnd
{
    public class SaveValidityChangesDTO
    {
        public int Period { get; set;  }

        public int CollectionYear { get; set; }

        public int HubPathId { get; set; }

        public int EntityType { get; set; }

        public bool? Enabled { get; set; }
    }
}
