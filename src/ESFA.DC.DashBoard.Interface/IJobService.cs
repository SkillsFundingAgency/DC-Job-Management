using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DashBoard.Models.Job;

namespace ESFA.DC.DashBoard.Interface
{
    public interface IJobService
    {
        Task<JobStatsModel> ProvideAsync(CancellationToken cancellationToken);
    }
}