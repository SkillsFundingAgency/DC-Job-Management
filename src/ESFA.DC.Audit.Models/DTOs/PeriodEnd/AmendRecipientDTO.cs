using System.Collections.Generic;
using ESFA.DC.EmailDistribution.Models;

namespace ESFA.DC.Audit.Models.DTOs.PeriodEnd
{
    public class AmendRecipientDTO
    {
        public int RecipientID { get; set;  }

        public string EmailAddress { get; set; }

        public List<RecipientGroupRecipient> RecipientGroupIds { get; set; }
    }
}
