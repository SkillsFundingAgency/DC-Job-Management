namespace ESFA.DC.PeriodEnd.Models
{
    public class PeriodEndJobState
    {
        public int Period { get; set; }

        public int PathId { get; set; }

        public int Position { get; set; }

        public int? Year { get; set; }

        public string CollectionName { get; set; }

        public bool IsBusy { get; set; }
    }
}