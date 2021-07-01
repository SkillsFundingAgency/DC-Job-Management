using System;

namespace ESFA.DC.CollectionsManagement.Models
{
    public class OrganisationCollection
    {
        public OrganisationCollection()
        {
        }

        public OrganisationCollection(long ukprn, int collectionId, int collectionTypeId, string collectionType, string collectionName, DateTime startDate, DateTime? endDate)
        {
            Ukprn = ukprn;
            CollectionId = collectionId;
            CollectionType = collectionType;
            CollectionTypeId = collectionTypeId;
            CollectionName = collectionName;
            StartDate = startDate;
            EndDate = endDate;
        }

        public long Ukprn { get; set; }

        public int CollectionId { get; set; }

        public string CollectionName { get; set; }

        public string CollectionType { get; set; }

        public int CollectionTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}