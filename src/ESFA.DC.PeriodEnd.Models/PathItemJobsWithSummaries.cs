using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Models
{
    public sealed class PathItemJobsWithSummaries
    {
        public PathItemJobsWithSummaries()
        {
            Jobs = new List<PathItemJobModel>();
            Summaries = new List<PathItemJobSummary>();
        }

        public List<PathItemJobModel> Jobs { get; set; }

        public List<PathItemJobSummary> Summaries { get; set; }
    }
}
