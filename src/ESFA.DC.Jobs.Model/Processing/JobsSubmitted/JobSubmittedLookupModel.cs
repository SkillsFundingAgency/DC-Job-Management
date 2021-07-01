using System;

namespace ESFA.DC.Jobs.Model.Processing.JobsSubmitted
{
    public class JobSubmittedLookupModel : ProcessingLookupModelBase
    {
        public DateTime CreatedDate { get; set; }

        public string FileName { get; set; }

        public int Status { get; set; }

        public string StatusDescription { get; set; }

        public string CollectionType { get; set; }
    }
}