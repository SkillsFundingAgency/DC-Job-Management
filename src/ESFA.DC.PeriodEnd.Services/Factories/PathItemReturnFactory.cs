using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Services.Factories
{
    public class PathItemReturnFactory : IPathItemReturnFactory
    {
        public PathItemReturn CreatePathTaskReturn(bool blockingTask, IEnumerable<long> jobIds, IEnumerable<int> subPaths)
        {
            return new PathItemReturn
            {
                BlockingTask = blockingTask,
                JobIds = jobIds,
                SubPaths = subPaths
            };
        }
    }
}