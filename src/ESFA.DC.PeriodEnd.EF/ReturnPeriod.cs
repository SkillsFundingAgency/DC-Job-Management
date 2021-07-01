using System;
using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF
{
    public partial class ReturnPeriod
    {
        public ReturnPeriod()
        {
            PeriodEnds = new HashSet<PeriodEnd>();
        }

        public int ReturnPeriodId { get; set; }
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime EndDateTimeUtc { get; set; }
        public int PeriodNumber { get; set; }
        public int CollectionId { get; set; }
        public int CalendarMonth { get; set; }
        public int CalendarYear { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual ICollection<PeriodEnd> PeriodEnds { get; set; }
    }
}
