using System;

namespace ESFA.DC.Audit.Models.DTOs.Providers
{
    public class EditProviderAssignmentsDTO
    {
        public int CollectionId { get; set; }

        public long? Ukprn { get; set; }

        public DateTime? StartDateUTC { get; set; }

        public DateTime? EndDateUTC { get; set; }
    }
}
