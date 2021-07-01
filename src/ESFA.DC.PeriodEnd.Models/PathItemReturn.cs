using System.Collections.Generic;

namespace ESFA.DC.PeriodEnd.Models
{
    public class PathItemReturn
    {
        public PathItemReturn()
        {
            SubPaths = new List<int>();
        }

        public IEnumerable<long> JobIds { get; set; }

        public bool BlockingTask { get; set; }

        public IEnumerable<int> SubPaths { get; set; }
    }
}