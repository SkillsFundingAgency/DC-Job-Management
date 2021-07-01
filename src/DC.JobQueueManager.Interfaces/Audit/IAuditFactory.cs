using System;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;

namespace ESFA.DC.JobQueueManager.Interfaces.Audit
{
    public interface IAuditFactory
    {
        IAudit BuildDataAudit<T>(Func<IJobQueueDataContext, Task<T>> auditFunc, IJobQueueDataContext context);

        IAudit BuildRequestAudit<T>(Func<T> auditFunc);
    }
}
