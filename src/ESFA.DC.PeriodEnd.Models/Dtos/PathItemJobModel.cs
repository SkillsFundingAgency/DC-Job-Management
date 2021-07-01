namespace ESFA.DC.PeriodEnd.Models.Dtos
{
    public class PathItemJobModel
    {
        public long JobId { get; set; }

        public int Status { get; set; }

        public int Ordinal { get; set; }

        public int Rank { get; set; }

        public string ProviderName { get; set; }

        public bool CanRetry { get; set; }
    }
}