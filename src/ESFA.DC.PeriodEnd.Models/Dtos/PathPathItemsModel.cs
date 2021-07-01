using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Models.Dtos
{
    public class PathPathItemsModel
    {
        public int PathId { get; set; }

        public string Name { get; set; }

        public int EntityType { get; set; }

        public int Position { get; set; }

        public bool IsBusy { get; set; }

        public PeriodEndValidityState IsValidForPeriod { get; set; }

        public bool IsPreviousPeriod { get; set; }

        public bool IsCritical { get; set; }

        public IEnumerable<PathItemModel> PathItems { get; set; }
    }
}