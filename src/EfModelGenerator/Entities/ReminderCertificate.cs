using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class ReminderCertificate
    {
        public int Id { get; set; }
        public int ReminderId { get; set; }
        public string Name { get; set; }
        public string Thumbprint { get; set; }

        public virtual Reminder Reminder { get; set; }
    }
}
