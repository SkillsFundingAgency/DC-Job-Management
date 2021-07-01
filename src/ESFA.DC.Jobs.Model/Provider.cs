using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.Jobs.Model
{
    public class Provider
    {
        public string Name { get; set; }

        public string TradingName { get; set; }

        public string Upin { get; set; }

        public long Ukprn { get; set; }

        public bool ExistsInSld { get; set; }

        public bool ActiveInPIMS { get; set; }

        public ProviderStatusType ProviderStatus { get; set; }

        public int StatusOrder { get; set; }
    }
}