using System;

namespace ESFA.DC.JobQueueManager.Interfaces.Audit
{
    public interface IAuditContext
    {
        string Username { get; }

        int Differentiator { get; }

        DateTime TimeStamp { get; }
    }
}
