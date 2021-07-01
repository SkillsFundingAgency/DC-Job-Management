using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IApiAvailabilityService
    {
        Task<bool> IsApiAvailableAsync(string apiName, CancellationToken cancellationToken);

        Task<bool> SetApiAvailabilityAsync(ApiAvailabilityDto apiAvailabilityDto, CancellationToken cancellationToken);
    }
}