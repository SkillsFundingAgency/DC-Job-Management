namespace ESFA.DC.Audit.Models.DTOs.PeriodEnd
{
    public class ClosedCollectionEmailDTO
    {
        public int? Year { get; set; }

        public int Period { get; set; }

        public string CollectionType { get; set; }

        public bool EmailSent { get; set; }
    }
}
