using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DashBoard.Models.ServiceBus;

namespace ESFA.DC.DashBoard.Interface
{
    public interface IServiceBusStatsService
    {
        Task<ServiceBusStatusModel> ProvideAsync(CancellationToken cancellationToken);
    }
}