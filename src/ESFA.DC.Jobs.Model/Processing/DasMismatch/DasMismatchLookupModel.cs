using System;

namespace ESFA.DC.Jobs.Model.Processing.DasMismatch
{
    public class DasMismatchLookupModel : ProcessingLookupModelBase
    {
        public string FileName { get; set; }

        public DateTime SubmissionDate { get; set; }
    }
}
