using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ReturnPeriod
    {
        public ReturnPeriod()
        {
            Covid19ReliefSubmission = new HashSet<Covid19ReliefSubmission>();
            PeriodEnd = new HashSet<PeriodEnd>();
            ReturnPeriodDisplayOverride = new HashSet<ReturnPeriodDisplayOverride>();
            ReturnPeriodOrganisationOverride = new HashSet<ReturnPeriodOrganisationOverride>();
        }

        public int ReturnPeriodId { get; set; }
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime EndDateTimeUtc { get; set; }
        public int PeriodNumber { get; set; }
        public int CollectionId { get; set; }
        public int CalendarMonth { get; set; }
        public int CalendarYear { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual ICollection<Covid19ReliefSubmission> Covid19ReliefSubmission { get; set; }
        public virtual ICollection<PeriodEnd> PeriodEnd { get; set; }
        public virtual ICollection<ReturnPeriodDisplayOverride> ReturnPeriodDisplayOverride { get; set; }
        public virtual ICollection<ReturnPeriodOrganisationOverride> ReturnPeriodOrganisationOverride { get; set; }
    }
}
