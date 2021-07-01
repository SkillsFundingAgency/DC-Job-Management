using System;
using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF.Console.Entities
{
    public partial class PeriodEnd
    {
        public PeriodEnd()
        {
            Paths = new HashSet<Path>();
        }

        public int PeriodEndId { get; set; }
        public int PeriodId { get; set; }
        public bool ProviderReportsReady { get; set; }
        public bool ProviderReportsPublished { get; set; }
        public bool McareportsReady { get; set; }
        public bool McareportsPublished { get; set; }
        public bool Closed { get; set; }
        public DateTime? PeriodEndStarted { get; set; }
        public DateTime? PeriodEndFinished { get; set; }
        public bool CollectionClosedEmailSent { get; set; }

        public virtual ReturnPeriod Period { get; set; }
        public virtual ICollection<Path> Paths { get; set; }
    }
}
