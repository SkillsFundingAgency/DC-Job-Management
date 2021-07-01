using System.Collections.Generic;

namespace ESFA.DC.EmailDistribution.Models
{
    public class Recipient
    {
        public int RecipientId { get; set; }

        public string EmailAddress { get; set; }

        public List<int> RecipientGroupIds { get; set; }
    }
}
