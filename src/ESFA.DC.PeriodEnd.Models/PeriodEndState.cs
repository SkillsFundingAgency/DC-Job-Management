using System;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Models
{
    public sealed class PeriodEndState : PeriodEndStateModel
    {
        public DateTime? EsfSummarisationFinished { get; set; }

        public DateTime? DcSummarisationFinished { get; set; }

        public DateTime? AppsSummarisationFinished { get; set; }
    }
}
