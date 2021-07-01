using System;

namespace ESFA.DC.JobQueueManager.Interfaces.Audit
{
    public interface IAuditContextProvider
    {
        void Configure(string username, int? differentiator, DateTime? timeStamp);

        IAuditContext Provide();
    }
}
