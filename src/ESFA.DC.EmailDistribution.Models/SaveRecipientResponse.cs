using System.Collections.Generic;

namespace ESFA.DC.EmailDistribution.Models
{
    public class SaveRecipientResponse
    {
        public SaveRecipientResponse()
        {
            RecipientGroups = new List<RecipientGroup>();
        }

        public IEnumerable<RecipientGroup> RecipientGroups { get; set; }

        public bool IsDuplicateEmail { get; set; }
    }
}
