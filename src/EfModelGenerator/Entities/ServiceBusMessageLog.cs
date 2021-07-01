using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ServiceBusMessageLog
    {
        public int Id { get; set; }
        public long JobId { get; set; }
        public string Message { get; set; }
        public DateTime DateTimeCreatedUtc { get; set; }
    }
}
