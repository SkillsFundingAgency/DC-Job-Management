using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces.Audit;

namespace ESFA.DC.JobQueueManager.Audit
{
    public class AuditRequest<T> : IAudit
    {
        private readonly Func<T> _auditFunc;
        private readonly IAuditRepository _auditRepository;
        private readonly IAuditContext _auditContext;

        private T _auditBefore;
        private T _auditAfter;

        public AuditRequest(Func<T> auditFunc, IAuditRepository auditRepository, IAuditContext auditContext)
        {
            _auditFunc = auditFunc;
            _auditRepository = auditRepository;
            _auditContext = auditContext;
        }

        public async Task BeforeAsync(CancellationToken cancellationToken)
        {
            if (ShouldAudit())
            {
                _auditBefore = _auditFunc();
            }
        }

        public async Task AfterAndSaveAsync(CancellationToken cancellationToken)
        {
            if (ShouldAudit())
            {
                _auditAfter = _auditFunc();
                await _auditRepository.Save(_auditContext, _auditBefore, _auditAfter, cancellationToken);
            }
        }

        private bool ShouldAudit()
        {
            return _auditContext != null;
        }
    }
}
