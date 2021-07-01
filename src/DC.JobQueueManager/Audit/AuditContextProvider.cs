using System;
using ESFA.DC.JobQueueManager.Interfaces.Audit;

namespace ESFA.DC.JobQueueManager.Audit
{
    public class AuditContextProvider : IAuditContextProvider
    {
        private IAuditContext _auditContext;

        public void Configure(string username, int? differentiator, DateTime? timeStamp)
        {
           if (username != null && differentiator.HasValue && timeStamp.HasValue)
           {
                _auditContext = new AuditContext(username, differentiator.Value, timeStamp.Value);
           }
        }

        public IAuditContext Provide()
        {
            return _auditContext;
        }
    }
}
