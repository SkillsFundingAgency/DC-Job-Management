using System;

namespace ESFA.DC.Data.Models
{
    public class SummarisationCollectionReturnCode
    {
        public string CurrentReturnCode { get; set; }

        public string CollectionType { get; set; }

        public string CollectionReturnCode { get; set; }

        public DateTime DateTime { get; set; }

        public int Id { get; set; }
    }
}