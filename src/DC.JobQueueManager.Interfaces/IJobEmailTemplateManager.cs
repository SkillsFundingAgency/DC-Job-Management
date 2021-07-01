using System;
using System.Threading.Tasks;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IJobEmailTemplateManager
    {
        Task<string> GetTemplate(long jobId, DateTime dateTimeJobSubmittedUtc);

        Task SendEmailNotification(long jobId);
    }
}
