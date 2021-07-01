using System;

namespace ESFA.DC.Jobs.Model
{
    public class Certificate
    {
        public int? CertificateId { get; set; }

        public int? ReminderId { get; set; }

        public string Name { get; set; }

        public string Thumbprint { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
