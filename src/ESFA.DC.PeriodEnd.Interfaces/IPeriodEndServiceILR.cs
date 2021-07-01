using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IPeriodEndServiceILR : IPeriodEndService
    {
        Task ToggleReferenceDataJobsAsync(bool pause, CancellationToken cancellationToken);

        Task<List<JobSchedule>> GetReferenceDataJobsAsync(CancellationToken cancellationToken);

        Task<List<ReportsPublished>> GetPublishedReportPeriodsAsync(CancellationToken cancellationToken);

        Task<bool> ClosePeriodEndAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken);
    }
}