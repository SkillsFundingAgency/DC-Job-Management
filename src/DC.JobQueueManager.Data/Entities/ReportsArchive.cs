using System;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ReportsArchive
    {
        public int Id { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedDateTimeUtc { get; set; }
        public int Year { get; set; }
        public int Period { get; set; }
        public int CollectionTypeId { get; set; }
        public bool InSld { get; set; }
        public long Ukprn { get; set; }
        public string FileName { get; set; }

        public virtual CollectionType CollectionType { get; set; }
    }
}
