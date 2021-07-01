using System;
using System.Collections.Generic;

namespace ESFA.DC.JobQueueManager.Data.Entities
{
    public partial class Reminder
    {
        public Reminder()
        {
            ReminderCertificate = new HashSet<ReminderCertificate>();
        }

        public int ReminderId { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public DateTime DeadlineDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public virtual ICollection<ReminderCertificate> ReminderCertificate { get; set; }
    }
}
