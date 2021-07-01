namespace ESFA.DC.Data.Models
{
    public class SummarisationTotal
    {
        public int CollectionReturnId { get; set; }

        public string CollectionType { get; set; }

        public string CollectionReturnCode { get; set; }

        public decimal TotalActualValue { get; set; }
    }
}
