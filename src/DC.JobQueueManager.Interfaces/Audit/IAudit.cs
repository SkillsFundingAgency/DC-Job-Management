using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.JobQueueManager.Interfaces.Audit
{
    public interface IAudit
    {
        Task BeforeAsync(CancellationToken cancellationToken);

        Task AfterAndSaveAsync(CancellationToken cancellationToken);
    }
}
