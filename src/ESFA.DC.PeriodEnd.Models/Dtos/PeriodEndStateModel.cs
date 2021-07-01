using System;
using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.Models.Dtos
{
    public class PeriodEndStateModel
    {
        public bool ProviderReportsPublished { get; set; }

        public bool ProviderReportsReady { get; set; }

        public bool Fm36ReportsPublished { get; set; }

        public bool Fm36ReportsReady { get; set; }

        public bool McaReportsPublished { get; set; }

        public bool McaReportsReady { get; set; }

        public bool PeriodEndStarted { get; set; }

        public bool PeriodEndFinished { get; set; }

        public bool CollectionClosed { get; set; }

        public bool ReferenceDataJobsPaused { get; set; }

        public bool CollectionClosedEmailSent { get; set; }

        public IEnumerable<PathPathItemsModel> Paths { get; set; }

        public bool IsInitialised { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
