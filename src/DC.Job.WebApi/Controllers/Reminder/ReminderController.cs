using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.Reminder
{
    [Produces("application/json")]
    [Route("api/reminder")]
    public class ReminderController : Controller
    {
        private IReminderService _reminderService;

        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpPost("upsert")]
        public async Task UpsertReminderAsync([FromBody] IEnumerable<Jobs.Model.Reminder> reminders, CancellationToken cancellationToken)
        {
            await _reminderService.UpsertReminderAsync(reminders, cancellationToken);
        }

        [HttpPost("certificates/add")]
        public async Task AddReminderCertificatesAsync([FromBody] IEnumerable<Jobs.Model.Certificate> certificates, CancellationToken cancellationToken)
        {
            await _reminderService.AddReminderCertificatesAsync(certificates, cancellationToken);
        }

        [HttpGet("all")]
        public async Task<IEnumerable<Jobs.Model.Reminder>> GetAllRemindersAsync(CancellationToken cancellationToken)
        {
            return await _reminderService.GetAllRemindersAsync(cancellationToken);
        }
    }
}
