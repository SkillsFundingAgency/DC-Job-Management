using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Models
{
    public sealed class PathItemJobSummary : PathItemJobSummaryModel
    {
        public int HubPathId { get; set; }

        public int Ordinal { get; set; }

        public string PathItemLabel { get; set; }
    }
}
