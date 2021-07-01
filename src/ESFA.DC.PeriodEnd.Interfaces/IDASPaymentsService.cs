using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IDASPaymentsService
    {
        Task<IEnumerable<DasPaymentsModel>> GetMissingDASPayments(
            IEnumerable<CurrentPeriodSuccessfulIlrModel> ilrJobs,
            int year,
            int collectionPeriod,
            CancellationToken cancellationToken);
    }
}