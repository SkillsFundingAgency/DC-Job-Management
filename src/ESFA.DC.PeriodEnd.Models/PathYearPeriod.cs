namespace ESFA.DC.PeriodEnd.Models
{
    public class PathYearPeriod
    {
        public int PathId { get; set; }

        public int HubPathId { get; set; }

        public int? Year { get; set; }

        public int Period { get; set; }

        public bool PeriodClosed { get; set; }
    }
}