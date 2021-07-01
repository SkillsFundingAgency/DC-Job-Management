using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IPeriodEndServiceALLF : IPeriodEndService
    {
        Task<IEnumerable<JobMetaDataDto>> GetFileUploadsForPeriodAsync(
            int collectionYear,
            int period,
            CancellationToken cancellationToken);
    }
}