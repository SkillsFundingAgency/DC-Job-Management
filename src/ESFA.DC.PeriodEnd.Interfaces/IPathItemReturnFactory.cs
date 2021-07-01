using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IPathItemReturnFactory
    {
        PathItemReturn CreatePathTaskReturn(bool blockingTask, IEnumerable<long> jobId, IEnumerable<int> subPaths);
    }
}