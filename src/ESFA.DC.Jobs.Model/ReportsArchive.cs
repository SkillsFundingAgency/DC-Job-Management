using System;

namespace ESFA.DC.Jobs.Model
{
    public class ReportsArchive
    {
        public string UploadedBy { get; set; }

        public DateTime UploadedDateTimeUtc { get; set; }

        public int Year { get; set; }

        public int Period { get; set; }

        public int CollectionTypeId { get; set; }

        public bool InSld { get; set; }

        public string CollectionType { get; set; }

        public string FileName { get; set; }
    }
}
