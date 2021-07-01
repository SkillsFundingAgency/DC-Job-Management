using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IReminderService
    {
        Task UpsertReminderAsync(IEnumerable<Reminder> reminders, CancellationToken cancellationToken);

        Task<IEnumerable<Reminder>> GetAllRemindersAsync(CancellationToken cancellationToken);

        Task AddReminderCertificatesAsync(IEnumerable<Certificate> certificates, CancellationToken cancellationToken);
    }
}
