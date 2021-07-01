using System;

namespace ESFA.DC.JobQueueManager.Data.ReadOnlyEntities
{
    public class ReadOnlyJobSubmitted : ReadOnlyJobBase
    {
        public DateTime CreatedDate { get; set; }

        public string FileName { get; set; }

        public int Status { get; set; }

        public string StatusDescription { get; set; }

        public string CollectionType { get; set; }
    }
}