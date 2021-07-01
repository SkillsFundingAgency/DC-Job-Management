namespace ESFA.DC.PeriodEnd.Models
{
    public class CollectionStats
    {
        public string CollectionName { get; set; }

        public int CountOfComplete { get; set; }

        public int CountOfFail { get; set; }

        public int Total { get; set; }

        public decimal Percent { get; set; }
    }
}