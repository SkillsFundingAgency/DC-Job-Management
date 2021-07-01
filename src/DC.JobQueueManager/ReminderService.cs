using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using Reminder = ESFA.DC.Jobs.Model.Reminder;

namespace ESFA.DC.JobQueueManager
{
    public class ReminderService : IReminderService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ReminderService(
            Func<IJobQueueDataContext> contextFactory,
            ILogger logger,
            IDateTimeProvider dateTimeProvider)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task UpsertReminderAsync(IEnumerable<Reminder> reminders, CancellationToken cancellationToken)
        {
            if (reminders == null || !reminders.Any())
            {
                _logger.LogInfo("Call made to UpsertReminderAsync with a null object");
                return;
            }

            try
            {
                using (var context = _contextFactory())
                {
                    foreach (var reminder in reminders)
                    {
                        var matchedReminder =
                            await context.Reminder.FirstOrDefaultAsync(s => s.ReminderId == reminder.ReminderId, cancellationToken) ??
                            new Data.Entities.Reminder();

                        matchedReminder.Description = reminder.Description;
                        matchedReminder.ReminderDate = reminder.ReminderDate;
                        matchedReminder.DeadlineDate = reminder.DeadlineDate;
                        matchedReminder.ClosedDate = reminder.ClosedDate;
                        matchedReminder.Notes = reminder.Notes;
                        matchedReminder.CreatedOn = _dateTimeProvider.GetNowUtc();
                        matchedReminder.UpdatedOn = reminder.UpdatedOn;
                        matchedReminder.CreatedBy = reminder.CreatedBy;
                        matchedReminder.UpdatedBy = reminder.UpdatedBy;

                        if (reminder.Certificates != null)
                        {
                            var matchedCertificate = await context.ReminderCertificate.FirstOrDefaultAsync(
                                s => s.Id == reminder.Certificates.First().CertificateId &&
                                     s.ReminderId == reminder.Certificates.First().ReminderId, cancellationToken);
                            if (matchedCertificate == null)
                            {
                                matchedCertificate = new ReminderCertificate();
                                matchedReminder.ReminderCertificate.Add(matchedCertificate);
                            }

                            matchedCertificate.Name = reminder.Certificates.First().Name;
                            matchedCertificate.Thumbprint = reminder.Certificates.First().Thumbprint;
                        }
                    }

                    await context.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error upserting reminders. {ex.Message}");
                throw;
            }
        }

        public async Task AddReminderCertificatesAsync(IEnumerable<Certificate> certificates, CancellationToken cancellationToken)
        {
            if (certificates == null || !certificates.Any())
            {
                _logger.LogInfo("Call made to AddCertificates with a null object");
                return;
            }

            try
            {
                using (var context = _contextFactory())
                {
                    var reminderCertificates = new List<ReminderCertificate>();

                    foreach (var certificate in certificates.Where(c =>
                        !context.ReminderCertificate.Any(a => a.Thumbprint == c.Thumbprint)))
                    {
                        reminderCertificates.Add(new ReminderCertificate
                        {
                            Name = certificate.Name,
                            Thumbprint = certificate.Thumbprint,
                            Reminder = new Data.Entities.Reminder
                            {
                                DeadlineDate = certificate.ExpiryDate.GetValueOrDefault(),
                                CreatedOn = _dateTimeProvider.GetNowUtc(),
                                CreatedBy = "System",
                            }
                        });
                    }

                    await context.ReminderCertificate.AddRangeAsync(reminderCertificates, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving reminder certificates. {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Reminder>> GetAllRemindersAsync(CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.Reminder.Include(i => i.ReminderCertificate).Select(s =>
                    new Reminder
                    {
                        ReminderId = s.ReminderId,
                        Description = s.Description,
                        ReminderDate = s.ReminderDate,
                        DeadlineDate = s.DeadlineDate,
                        ClosedDate = s.ClosedDate,
                        Notes = s.Notes,
                        UpdatedOn = s.UpdatedOn,
                        CreatedBy = s.CreatedBy,
                        UpdatedBy = s.UpdatedBy,
                        Certificates = s.ReminderCertificate.Select(c => new Certificate
                        {
                            ReminderId = s.ReminderId,
                            CertificateId = c.Id,
                            Name = c.Name,
                            Thumbprint = c.Thumbprint
                        }).ToList()
                    }).ToListAsync(cancellationToken);
            }
        }
    }
}
