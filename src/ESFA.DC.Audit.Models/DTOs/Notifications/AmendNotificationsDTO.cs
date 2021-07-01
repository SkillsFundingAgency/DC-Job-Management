using System;

namespace ESFA.DC.Audit.Models.DTOs.Notifications
{
    public class AmendNotificationsDTO
    {
        public int Id { get; set;  }

        public string Headline { get; set;  }

        public string Message { get; set; }

        public DateTime StartDateUTC { get; set;  }

        public DateTime? EndDateUTC { get; set;  }

        public bool IsEnabled { get; set; }
    }
}