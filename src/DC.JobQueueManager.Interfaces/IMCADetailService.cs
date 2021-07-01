using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IMcaDetailService
    {
        Task<string> GetGLACodeAsync(long ukprn);

        Task<long?> GetLastProcessedJobIdAsync(long ukprn, int period, int collectionYear);

        Task<IEnumerable<long>> GetActiveMcaListAsync(int collectionYear, DateTime periodEndDate);

        Task<IEnumerable<string>> GetGLACodesAsync();
    }
}
