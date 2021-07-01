using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.JobScheduler.Interfaces
{
    public interface IServiceBusMessageLogger
    {
        Task LogMessageAsync(IJobContextMessage message, CancellationToken cancellationToken);
    }
}
