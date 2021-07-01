using System;

namespace ESFA.DC.Jobs.Model
{
    public class FailedJob
    {
        public long JobId { get; set; }

        public DateTime? DateTimeSubmitted { get; set; }

        public long Ukprn { get; set; }

        public string OrganisationName { get; set; }

        public string CollectionName { get; set; }

        public string FileName { get; set; }

        public string CollectionType { get; set; }

        public short Status { get; set; }
    }
}