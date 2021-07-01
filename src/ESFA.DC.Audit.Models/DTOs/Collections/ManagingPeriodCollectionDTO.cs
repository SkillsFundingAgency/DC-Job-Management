using System;

namespace ESFA.DC.Audit.Models.DTOs.Collections
{
    public class ManagingPeriodCollectionDTO
    {
        public int ReturnPeriodId { get; set; }

        public DateTime OpeningDateUTC { get; set;  }

        public DateTime ClosingDateUTC { get; set;  }
    }
}