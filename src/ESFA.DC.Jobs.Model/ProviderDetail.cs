using System.Collections.Generic;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.Jobs.Model
{
    public class ProviderDetail
    {
        public string Name { get; set; }

        public string Upin { get; set; }

        public long Ukprn { get; set; }

        public bool IsMCA { get; set; }

        public bool ActiveInPIMS { get; set; }

        public ProviderStatusType ProviderStatus { get; set; }

        public bool HasFundingClaims { get; set; }

        public IEnumerable<ProviderLatestSubmission> ProviderLatestSubmissions { get; set; }
    }
}
