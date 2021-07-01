using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Audit.Models.DTOs.Notifications;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class ServiceMessageService : IServiceMessageService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly ILogger _logger;
        private readonly IAuditFactory _auditFactory;

        public ServiceMessageService(
            Func<IJobQueueDataContext> contextFactory,
            ILogger logger,
            IAuditFactory auditFactory)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _auditFactory = auditFactory;
        }

        public async Task<string> GetMessageAsync(DateTime dateTimeUtc, string controllerName, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var data = await context
                        .ServiceMessage
                        .OrderByDescending(sm => sm.StartDateTimeUtc)
                        .FirstOrDefaultAsync(
                            x => x.Enabled &&
                                 dateTimeUtc >= x.StartDateTimeUtc &&
                                 (!x.EndDateTimeUtc.HasValue || dateTimeUtc <= x.EndDateTimeUtc) &&
                                 x.ServicePageMessage.Any(spm => spm.Page.ControllerName == controllerName),
                            cancellationToken);

                    return data?.Message;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting service message for : {dateTimeUtc}", e);
                throw;
            }
        }

        public async Task<IEnumerable<ServicePageDto>> GetAllPagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var data = await context.ServicePage
                        .Select(sp => new ServicePageDto
                        {
                            DisplayName = sp.DisplayName,
                            ControllerName = sp.ControllerName
                        })
                        .ToListAsync(cancellationToken);

                    return data;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting service pages", e);
                throw;
            }
        }

        public async Task<ServiceMessageDto> GetMessageAsync(int messageId, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var data = await context
                        .ServiceMessage
                        .Include(sm => sm.ServicePageMessage)
                        .ThenInclude(spm => spm.Page)
                        .SingleOrDefaultAsync(x => x.Id == messageId, cancellationToken);

                    return ConvertServiceMessageToDto(data);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting service message for id: {messageId}", e);
                throw;
            }
        }

        public async Task DeleteMessageAsync(int messageId, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var audit = _auditFactory.BuildDataAudit(await BuildNotificationsDeletionFunc(messageId), context);
                    await audit.BeforeAsync(cancellationToken);

                    var messagePages = context.ServicePageMessage
                        .Where(spm => spm.MessageId == messageId)
                        .ToList();

                    foreach (var page in messagePages)
                    {
                        context.ServicePageMessage.Remove(page);
                    }

                    var message = await context
                        .ServiceMessage
                        .SingleOrDefaultAsync(x => x.Id == messageId, cancellationToken);

                    context.ServiceMessage.Remove(message);

                    await context.SaveChangesAsync(cancellationToken);
                    await audit.AfterAndSaveAsync(cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting service message for id: {messageId}", e);
                throw;
            }
        }

        public async Task<bool> SaveMessageAsync(ServiceMessageDto serviceMessage, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var audit = _auditFactory.BuildDataAudit(await BuildNotificationsFunc(serviceMessage), context);
                    await audit.BeforeAsync(cancellationToken);
                    ServiceMessage entity;
                    if (serviceMessage.Id > 0)
                    {
                        entity = await context.ServiceMessage
                            .SingleOrDefaultAsync(x => x.Id == serviceMessage.Id, cancellationToken);
                    }
                    else
                    {
                        entity = new ServiceMessage();
                        context.ServiceMessage.Add(entity);
                    }

                    entity.StartDateTimeUtc = serviceMessage.StartDateTimeUtc;
                    entity.EndDateTimeUtc = serviceMessage.EndDateTimeUtc;
                    entity.Headline = serviceMessage.Headline;
                    entity.Message = serviceMessage.Message;
                    entity.Enabled = serviceMessage.IsEnabled;

                    await context.SaveChangesAsync(cancellationToken);

                    var existingPages = context.ServicePageMessage
                        .Where(spm => spm.MessageId == serviceMessage.Id)
                        .ToList();

                    context.ServicePageMessage.RemoveRange(existingPages);

                    if (serviceMessage.Pages != null)
                    {
                        var pageNames = serviceMessage.Pages.Select(p => p.DisplayName).ToList();
                        var pageEntities = context.ServicePage.Where(sp => pageNames.Contains(sp.DisplayName)).ToList();

                        foreach (var page in serviceMessage.Pages)
                        {
                            await context.ServicePageMessage.AddAsync(
                                new ServicePageMessage
                                {
                                    MessageId = entity.Id,
                                    PageId = pageEntities.First(p => p.DisplayName == page.DisplayName).Id
                                },
                                cancellationToken);
                        }
                    }

                    await context.SaveChangesAsync(cancellationToken);
                    await audit.AfterAndSaveAsync(cancellationToken);
                    return true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error saving service message", e);
                throw;
            }
        }

        public async Task<IEnumerable<ServiceMessageDto>> GetAllMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var data = await context
                        .ServiceMessage
                        .Include(sm => sm.ServicePageMessage)
                        .ThenInclude(spm => spm.Page)
                        .OrderByDescending(o => o.Enabled)
                        .ThenByDescending(o => o.StartDateTimeUtc)
                        .ToListAsync(cancellationToken);

                    return data.Select(ConvertServiceMessageToDto);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting service messages", e);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetControllerNamesThatDisplayMessagesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    var data = await context
                        .ServicePageMessage
                        .Select(spm => spm.Page.ControllerName)
                        .Distinct()
                        .ToListAsync(cancellationToken);

                    return data;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error getting controller names", e);
                throw;
            }
        }

        private ServiceMessageDto ConvertServiceMessageToDto(ServiceMessage serviceMessage)
        {
            return new ServiceMessageDto
            {
                Id = serviceMessage.Id,
                Headline = serviceMessage.Headline,
                Message = serviceMessage.Message,
                StartDateTimeUtc = serviceMessage.StartDateTimeUtc,
                EndDateTimeUtc = serviceMessage.EndDateTimeUtc,
                IsEnabled = serviceMessage.Enabled,
                Pages = serviceMessage.ServicePageMessage
                    .Select(spm => new ServicePageDto
                {
                    ControllerName = spm.Page?.ControllerName,
                    DisplayName = spm.Page?.DisplayName
                }).ToList()
            };
        }

        private async Task<Func<IJobQueueDataContext, Task<AmendNotificationsDTO>>> BuildNotificationsFunc(ServiceMessageDto dto)
        {
            return async c => await c.ServiceMessage
                .Select(s => new AmendNotificationsDTO
                {
                    Id = s.Id,
                    Headline = s.Headline,
                    Message = s.Message,
                    StartDateUTC = s.StartDateTimeUtc,
                    EndDateUTC = s.EndDateTimeUtc,
                    IsEnabled = s.Enabled
                }).FirstOrDefaultAsync(s => s.Headline == dto.Headline
                                             && s.Message == dto.Message
                                             && s.StartDateUTC == dto.StartDateTimeUtc
                                             && s.EndDateUTC == dto.EndDateTimeUtc);
        }

        private async Task<Func<IJobQueueDataContext, Task<AmendNotificationsDTO>>> BuildNotificationsDeletionFunc(int id)
        {
            return async c => await c.ServiceMessage
                .Select(s => new AmendNotificationsDTO
                {
                    Id = s.Id,
                    Headline = s.Headline,
                    Message = s.Message,
                    StartDateUTC = s.StartDateTimeUtc,
                    EndDateUTC = s.EndDateTimeUtc,
                    IsEnabled = s.Enabled
                }).SingleOrDefaultAsync(s => s.Id == id);
        }
    }
}
