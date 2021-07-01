namespace ESFA.DC.PeriodEnd.Models
{
    public class ProviderJob
    {
        public long JobId { get; set; }

        public int UkPrn { get; set; }

        public string FileName { get; set; }

        public string StorageReference { get; set; }
    }
}