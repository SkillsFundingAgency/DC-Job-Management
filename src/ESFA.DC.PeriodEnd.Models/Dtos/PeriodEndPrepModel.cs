using System.Collections.Generic;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.PeriodEnd.Models.Dtos
{
    public sealed class PeriodEndPrepModel
    {
        public List<JobSchedule> ReferenceDataJobs { get; set; }

        public List<FailedJob> FailedJobs { get; set; }

        public PeriodEndStateModel State { get; set; }

        public List<McaDetails> McaDetails { get; set; }

        public int SLDDASMismatches { get; set; }
    }
}