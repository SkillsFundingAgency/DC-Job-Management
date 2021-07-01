using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces.Audit;

namespace ESFA.DC.JobQueueManager.Audit
{
    public class AuditData<T> : IAudit
    {
        private readonly Func<IJobQueueDataContext, Task<T>> _auditFunc;
        private readonly IJobQueueDataContext _context;
        private readonly IAuditRepository _auditRepository;
        private readonly IAuditContext _auditContext;

        private T _auditBefore;
        private T _auditAfter;

        public AuditData(Func<IJobQueueDataContext, Task<T>> auditFunc, IJobQueueDataContext context, IAuditRepository auditRepository, IAuditContext auditContext)
        {
            _auditFunc = auditFunc;
            _context = context;
            _auditRepository = auditRepository;
            _auditContext = auditContext;
        }

        public async Task BeforeAsync(CancellationToken cancellationToken)
        {
            if (ShouldAudit())
            {
                _auditBefore = await _auditFunc(_context);
            }
        }

        public async Task AfterAndSaveAsync(CancellationToken cancellationToken)
        {
            if (ShouldAudit())
            {
                _auditAfter = await _auditFunc(_context);
                await _auditRepository.Save(_auditContext, _auditBefore, _auditAfter, cancellationToken);
            }
        }

        private bool ShouldAudit()
        {
            return _auditContext != null;
        }
    }
}