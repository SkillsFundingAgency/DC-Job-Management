using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IFailedJobQueryService
    {
        Task<List<FailedJob>> GetFailedJobsPerPeriod(int collectionYear, int period);
    }
}
