using System;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class Mcadetail
    {
        public int Id { get; set; }
        public long Ukprn { get; set; }
        public string Glacode { get; set; }
        public int Sofcode { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public int AcademicYearFrom { get; set; }
        public int? AcademicYearTo { get; set; }

    }
}