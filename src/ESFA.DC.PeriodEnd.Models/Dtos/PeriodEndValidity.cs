using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.Models.Dtos
{
    public class PeriodEndValidity
    {
        public bool PeriodEndHasRunForPeriod { get; set; }

        public bool PeriodEndIsRunning { get; set; }

        public List<PathPathItemsModel> Paths { get; set; }
    }
}