using System;
using ESFA.DC.JobQueueManager.Interfaces.Audit;

namespace ESFA.DC.JobQueueManager.Audit
{
    public class AuditContext : IAuditContext
    {
        public AuditContext(string username, int differentiator, DateTime timestamp)
        {
            Username = username;
            Differentiator = differentiator;
            TimeStamp = timestamp;
        }

        public string Username { get; }

        public int Differentiator { get; }

        public DateTime TimeStamp { get; }
    }
}
