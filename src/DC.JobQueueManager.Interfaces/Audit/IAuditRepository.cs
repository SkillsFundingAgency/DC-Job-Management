using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.JobQueueManager.Interfaces.Audit
{
    public interface IAuditRepository
    {
        Task Save<T>(IAuditContext context, T before, T after, CancellationToken cancellationToken);
    }
}
