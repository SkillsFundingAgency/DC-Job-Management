namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class CovidReliefPayment
    {
        public long Ukprn { get; set; }
        public decimal? AebIlrEarningsR09 { get; set; }
        public decimal? AebIlrEarningsR10 { get; set; }
        public decimal? AebIlrEarningsR11 { get; set; }
        public decimal? NlappsIlrEarningsR09 { get; set; }
        public decimal? NlappsIlrEarningsR10 { get; set; }
        public decimal? NlappsIlrEarningsR11 { get; set; }
        public decimal? AebPrsPaymentR09 { get; set; }
        public decimal? AebPrsPaymentR10 { get; set; }
        public decimal? AebPrsPaymentR11 { get; set; }
        public decimal? NlappsPrsPaymentR09 { get; set; }
        public decimal? NlappsPrsPaymentR10 { get; set; }
        public decimal? NlappsPrsPaymentR11 { get; set; }
    }
}
