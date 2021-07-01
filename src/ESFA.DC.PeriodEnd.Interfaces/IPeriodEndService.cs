using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IPeriodEndService
    {
        Task ProceedAsync(int collectionYear, int periodNumber, int pathId, CancellationToken cancellationToken);

        Task ProceedAsync(long jobId = 0, CancellationToken cancellationToken = default(CancellationToken));

        Task InitialisePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken);

        Task StartPeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken);

        Task CollectionClosedEmailSentAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken);

        Task ClosePeriodEndAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken);

        Task<PeriodEndPrepModel> GetPrepStateAsync(int? collectionYear, int? period, string collectionType, CancellationToken cancellationToken);

        Task<PeriodEndStateModel> GetPathsStateAsync(int? collectionYear, int? periodNumber, string collectionType, CancellationToken cancellationToken);
    }
}