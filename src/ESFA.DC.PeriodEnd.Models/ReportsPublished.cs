namespace ESFA.DC.PeriodEnd.Models
{
    public class ReportsPublished
    {
        public int CollectionYear { get; set; }

        public int Period { get; set; }

        public bool ProviderReportsPublished { get; set; }

        public bool Fm36ReportsPublished { get; set; }

        public bool McaReportsPublished { get; set; }

        public bool FrmReportsPublished { get; set; }
    }
}