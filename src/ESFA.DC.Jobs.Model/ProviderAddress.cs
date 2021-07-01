using System;

namespace ESFA.DC.Jobs.Model
{
    public class ProviderAddress
    {
        public string Name { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string Address4 { get; set; }

        public string Town { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }

        public string Address => string.Join(Environment.NewLine, Address1, Address2, Address3, Address4, Town, Postcode, Country);
    }
}