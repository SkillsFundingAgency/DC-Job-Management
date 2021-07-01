using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Models.Dtos
{
    public class PathItemModel
    {
        public List<PathItemJobModel> PathItemJobs { get; set; }

        public int PathId { get; set; }

        public int PathItemId { get; set; }

        public PeriodEndEntityType EntityType { get; set; }

        public string Name { get; set; }

        public int Ordinal { get; set; }

        public bool IsPausing { get; set; }

        public bool HasJobs { get; set; }

        public bool Hidden { get; set; }

        public bool IsInitiating { get; set; }

        public bool IsProviderReport { get; set; }

        public bool DisplayAllJobs { get; set; }

        public PeriodEndValidityState IsValidForPeriod { get; set; }

        public IEnumerable<int> SubPaths { get; set; }

        public PathItemJobSummaryModel PathItemJobSummary { get; set; }
    }
}