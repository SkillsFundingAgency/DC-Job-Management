using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class Covid19ReliefNlappsMonthlyCap
    {
        [Key]
        public long Ukprn { get; set; }

        public string ProviderName { get; set; }

        public string EligibleApps { get; set; }

        [Column("1920mcv")]
        public string _1920mcv { get; set; }

        [Column("2021mcv")]
        public string _2021mcv { get; set; }

        public string Earningsjuly19 { get; set; }

        public string Earningsaug19 { get; set; }

        public string Earningssept19 { get; set; }

        public string Earningsoct19 { get; set; }

        public string Julyearning1920mcv { get; set; }

        public string Augearning1920mcv { get; set; }

        public string Septearning1920mcv { get; set; }

        public string Octearning1920mcv { get; set; }

        public string Monthlycapjuly { get; set; }

        public string Monthlycapaug { get; set; }

        public string Monthlycapsept { get; set; }

        public string Monthlycapoct { get; set; }

        public string July1920v2021check { get; set; }

        public string Aug1920v2021check { get; set; }

        public string Sept1920v2021check { get; set; }

        public string Oct1920v2021check { get; set; }

        public string Julyok { get; set; }

        public string Augok { get; set; }

        public string Septok { get; set; }

        public string Octok { get; set; }
    }
}