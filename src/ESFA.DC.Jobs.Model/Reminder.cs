using System;
using System.Collections.Generic;

namespace ESFA.DC.Jobs.Model
{
    public class Reminder
    {
        public int? ReminderId { get; set; }

        public string Description { get; set; }

        public DateTime? ReminderDate { get; set; }

        public DateTime DeadlineDate { get; set; }

        public DateTime? ClosedDate { get; set; }

        public string Notes { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public IEnumerable<Certificate> Certificates { get; set; }
    }
}
