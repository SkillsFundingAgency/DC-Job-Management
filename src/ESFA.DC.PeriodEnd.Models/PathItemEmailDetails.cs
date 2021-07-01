using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.Models
{
    public class PathItemEmailDetails
    {
        public string TemplateId { get; set; }

        public IEnumerable<string> Recipients { get; set; }
    }
}