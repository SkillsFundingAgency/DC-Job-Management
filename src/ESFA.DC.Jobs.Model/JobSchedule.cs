using System;

namespace ESFA.DC.Jobs.Model
{
    public class JobSchedule
    {
        public string DisplayName { get; set; }

        public string Status { get; set; }

        public DateTime? NextRunDue { get; set; }
    }
}