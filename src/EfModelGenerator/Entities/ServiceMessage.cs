using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ServiceMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool Enabled { get; set; }
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime? EndDateTimeUtc { get; set; }
        public string Headline { get; set; }
    }
}
