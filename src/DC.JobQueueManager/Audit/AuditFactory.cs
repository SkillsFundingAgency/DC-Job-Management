using System;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces.Audit;

namespace ESFA.DC.JobQueueManager.Audit
{
    public class AuditFactory : IAuditFactory
    {
        private readonly IAuditContextProvider _auditContextProvider;
        private readonly IAuditRepository _auditRepository;

        public AuditFactory(IAuditContextProvider auditContextProvider, IAuditRepository auditRepository)
        {
            _auditContextProvider = auditContextProvider;
            _auditRepository = auditRepository;
        }

        public IAudit BuildDataAudit<T>(Func<IJobQueueDataContext, Task<T>> auditFunc, IJobQueueDataContext context)
        {
            var auditContext = ProvideAuditContext();

            return new AuditData<T>(auditFunc, context, _auditRepository, auditContext);
        }

        public IAudit BuildRequestAudit<T>(Func<T> auditFunc)
        {
            var auditContext = ProvideAuditContext();

            return new AuditRequest<T>(auditFunc, _auditRepository, auditContext);
        }

        private IAuditContext ProvideAuditContext()
        {
            return _auditContextProvider.Provide();
        }
    }
}
