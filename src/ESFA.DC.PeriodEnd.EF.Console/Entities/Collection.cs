using System;
using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.EF.Console.Entities
{
    public partial class Collection
    {
        public Collection()
        {
            Jobs = new HashSet<Job>();
            ReturnPeriods = new HashSet<ReturnPeriod>();
            Schedules = new HashSet<Schedule>();
            ValidityPeriods = new HashSet<ValidityPeriod>();
        }

        public int CollectionId { get; set; }
        public string Name { get; set; }
        public bool IsOpen { get; set; }
        public int CollectionTypeId { get; set; }
        public int? CollectionYear { get; set; }
        public string Description { get; set; }
        public string SubText { get; set; }
        public bool? CrossloadingEnabled { get; set; }
        public bool? ProcessingOverrideFlag { get; set; }
        public bool MultiStageProcessing { get; set; }
        public string StorageReference { get; set; }

        public virtual CollectionType CollectionType { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
        public virtual ICollection<ReturnPeriod> ReturnPeriods { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<ValidityPeriod> ValidityPeriods { get; set; }
    }
}
