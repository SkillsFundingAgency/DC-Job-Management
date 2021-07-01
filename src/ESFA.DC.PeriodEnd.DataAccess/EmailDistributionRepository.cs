using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Audit.Models.DTOs.PeriodEnd;
using ESFA.DC.EmailDistribution.Models;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using Microsoft.EntityFrameworkCore;
using RecipientGroup = ESFA.DC.EmailDistribution.Models.RecipientGroup;
using RecipientGroupRecipient = ESFA.DC.JobQueueManager.Data.Entities.RecipientGroupRecipient;

namespace ESFA.DC.PeriodEnd.DataAccess
{
    public class EmailDistributionRepository : IEmailDistributionRepository
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IAuditFactory _auditFactory;

        public EmailDistributionRepository(
            ILogger logger,
            Func<IJobQueueDataContext> contextFactory,
            IAuditFactory auditFactory)
        {
            _contextFactory = contextFactory;
            _auditFactory = auditFactory;
        }

        public async Task<IEnumerable<EmailDistribution.Models.Recipient>> GetEmailDistributionGroupRecipients(int recipientGroupId)
        {
            var result = new List<EmailDistribution.Models.Recipient>();
            using (var context = _contextFactory())
            {
                var data = await context.RecipientGroupRecipient.Include(x => x.Recipient).Where(x => x.RecipientGroupId == recipientGroupId).ToListAsync();

                foreach (var recipient in data)
                {
                    result.Add(new EmailDistribution.Models.Recipient
                    {
                        EmailAddress = recipient.Recipient.EmailAddress,
                        RecipientId = recipient.RecipientId,
                    });
                }
            }

            return result;
        }

        public async Task<bool> RemoveRecipient(int recipientId, int recipientGroupId)
        {
            using (var context = _contextFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await ProvideRemoveRecipentDTOFunc(recipientId), context);
                await audit.BeforeAsync(CancellationToken.None);

                var recipientGroupRecipient = await context.RecipientGroupRecipient.SingleAsync(x => x.RecipientId == recipientId && x.RecipientGroupId == recipientGroupId);
                context.RecipientGroupRecipient.Remove(recipientGroupRecipient);

                // Check if there are other groups attached, otherwise delete the recipient too
                if (!context.RecipientGroupRecipient.Any(x => x.RecipientId != recipientId))
                {
                    var recipient = await context.Recipient.SingleAsync(x => x.RecipientId == recipientId);
                    context.Recipient.Remove(recipient);
                }

                await context.SaveChangesAsync();
                await audit.AfterAndSaveAsync(CancellationToken.None);
            }

            return true;
        }

        public async Task<RecipientGroup> GetEmailDistributionGroup(int recipientGroupId)
        {
            var result = new RecipientGroup();
            using (var context = _contextFactory())
            {
                var data = await context.RecipientGroup.SingleOrDefaultAsync(x => x.RecipientGroupId == recipientGroupId);

                if (data != null)
                {
                    result = new RecipientGroup
                    {
                        GroupName = data.GroupName,
                        RecipientGroupId = recipientGroupId
                    };
                }
            }

            return result;
        }

        public async Task<IEnumerable<RecipientGroup>> GetEmailDistributionGroups()
        {
            var result = new List<RecipientGroup>();
            using (var context = _contextFactory())
            {
                var data = await context.RecipientGroup.ToListAsync();

                foreach (var group in data)
                {
                    result.Add(new RecipientGroup
                    {
                        GroupName = group.GroupName,
                        RecipientGroupId = group.RecipientGroupId
                    });
                }
            }

            return result;
        }

        public async Task<IList<RecipientGroup>> GetEmailDistributionGroupsAsync(string recipientEmail, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.RecipientGroupRecipient.Include(x => x.RecipientGroup)
                                                                .Where(x => x.Recipient.EmailAddress.Equals(recipientEmail))
                                                                .Select(recipientGroupRecipient => new RecipientGroup
                                                                {
                                                                    GroupName = recipientGroupRecipient.RecipientGroup.GroupName,
                                                                    RecipientGroupId = recipientGroupRecipient.RecipientGroupId
                                                                }).ToListAsync(cancellationToken);
            }
        }

        public async Task<EmailTemplate> GetTemplate(int emailId)
        {
            var result = new EmailTemplate();
            using (var context = _contextFactory())
            {
                var value = await context.Email.Include(x => x.EmailRecipientGroup)
                    .ThenInclude(x => x.RecipientGroup)
                    .SingleOrDefaultAsync(x => x.EmailId == emailId);

                if (value != null)
                {
                    result = new EmailTemplate()
                    {
                        HubEmailId = value.HubEmailId,
                        TriggerPointName = value.TriggerPointName,
                        TemplateId = value.TemplateId,
                        TemplateName = value.TemplateName,
                        EmailId = value.EmailId,
                        RecipientGroup = value.EmailRecipientGroup.Select(x =>
                            new RecipientGroup
                            {
                                GroupName = x.RecipientGroup.GroupName,
                                RecipientGroupId = x.RecipientGroup.RecipientGroupId
                            }).FirstOrDefault()
                    };
                }

                result.RecipientGroup = result.RecipientGroup ?? new RecipientGroup();
                return result;
            }
        }

        public async Task<bool> SaveTemplate(EmailTemplate emailTemplate)
        {
            var result = new List<EmailTemplate>();
            using (var context = _contextFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await ProvideEditEmailTemplateDTOFunc(emailTemplate), context);
                await audit.BeforeAsync(CancellationToken.None);
                var data = await context.Email.Include(x => x.EmailRecipientGroup).SingleOrDefaultAsync(x => x.EmailId == emailTemplate.EmailId);

                if (data != null)
                {
                    context.EmailRecipientGroup.RemoveRange(data.EmailRecipientGroup);
                }

                context.EmailRecipientGroup.Add(new EmailRecipientGroup()
                {
                    RecipientGroupId = emailTemplate.RecipientGroup.RecipientGroupId,
                    EmailId = emailTemplate.EmailId,
                });
                await context.SaveChangesAsync();
                await audit.AfterAndSaveAsync(CancellationToken.None);
            }

            return true;
        }

        public async Task<IEnumerable<EmailTemplate>> GetEmailTemplates()
        {
            var result = new List<EmailTemplate>();
            using (var context = _contextFactory())
            {
                var data = await context.Email.Include(x => x.EmailRecipientGroup)
                    .ThenInclude(x => x.RecipientGroup)
                    .ToListAsync();

                foreach (var value in data)
                {
                    result.Add(new EmailTemplate
                    {
                        HubEmailId = value.HubEmailId,
                        TriggerPointName = value.TriggerPointName,
                        TemplateId = value.TemplateId,
                        TemplateName = value.TemplateName,
                        EmailId = value.EmailId,
                        RecipientGroup = value.EmailRecipientGroup.Select(x =>
                            new RecipientGroup
                            {
                                GroupName = x.RecipientGroup.GroupName,
                                RecipientGroupId = x.RecipientGroup.RecipientGroupId
                            }).FirstOrDefault()
                    });
                }
            }

            return result;
        }

        public async Task<bool> RemoveGroup(int recipientGroupId)
        {
            using (var context = _contextFactory())
            {
                // if there is any template attached to the group dont allow delete
                if (context.EmailRecipientGroup.Any(x => x.RecipientGroupId == recipientGroupId))
                {
                    return false;
                }

                var audit = _auditFactory.BuildDataAudit(await ProvideRemoveGroupDTOFunc(recipientGroupId), context);
                await audit.BeforeAsync(CancellationToken.None);
                var recipientGroupRecipients = await context.RecipientGroupRecipient.Where(x => x.RecipientGroupId == recipientGroupId).ToListAsync();
                var group = await context.RecipientGroup.SingleAsync(x => x.RecipientGroupId == recipientGroupId);

                context.RecipientGroupRecipient.RemoveRange(recipientGroupRecipients);
                context.RecipientGroup.Remove(group);

                await context.SaveChangesAsync();
                await audit.AfterAndSaveAsync(CancellationToken.None);
            }

            return true;
        }

        public async Task<bool> SaveGroup(string groupName)
        {
            using (var context = _contextFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await ProvideSaveGroupDTOFunc(groupName), context);
                context.RecipientGroup.Add(new JobQueueManager.Data.Entities.RecipientGroup
                {
                    GroupName = groupName
                });

                await context.SaveChangesAsync();
                await audit.AfterAndSaveAsync(CancellationToken.None);
            }

            return true;
        }

        public async Task<bool> SaveRecipientAsync(EmailDistribution.Models.Recipient recipient, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await ProvideSaveRecipentDTOFunc(recipient.EmailAddress), context);
                await audit.BeforeAsync(cancellationToken);

                var recipientEntity =
                await context.Recipient.SingleOrDefaultAsync(r => r.EmailAddress == recipient.EmailAddress, cancellationToken);

                if (recipientEntity == null)
                { // Not already on the Db, create a new one
                    recipientEntity = new JobQueueManager.Data.Entities.Recipient { EmailAddress = recipient.EmailAddress };
                    await context.Recipient.AddAsync(recipientEntity, cancellationToken);
                }

                var taskArray = recipient.RecipientGroupIds.Select(s => context.RecipientGroupRecipient.AddAsync(new RecipientGroupRecipient { Recipient = recipientEntity, RecipientGroupId = s }, cancellationToken)).ToArray();

                await Task.WhenAll(taskArray);

                await context.SaveChangesAsync(cancellationToken);
                await audit.AfterAndSaveAsync(cancellationToken);
            }

            return true;
        }

        private async Task<Func<IJobQueueDataContext, Task<EditEmailTemplateDTO>>> ProvideEditEmailTemplateDTOFunc(EmailTemplate emailTemplate)
        {
            return
                async c => await c.EmailRecipientGroup
                    .Select(s => new EditEmailTemplateDTO()
                    {
                        EmailId = s.EmailId,
                        EmailTemplateName = s.Email.TemplateName,
                        GroupId = s.RecipientGroupId
                    }).SingleOrDefaultAsync(s => s.EmailId == emailTemplate.EmailId);
        }

        private async Task<Func<IJobQueueDataContext, Task<AmendGroupDTO>>> ProvideSaveGroupDTOFunc(string groupName)
        {
            return
                async c => await c.RecipientGroup
                    .Select(s => new AmendGroupDTO()
                    {
                        GroupId = s.RecipientGroupId,
                        GroupName = s.GroupName
                    }).SingleOrDefaultAsync(s => s.GroupName == groupName);
        }

        private async Task<Func<IJobQueueDataContext, Task<AmendGroupDTO>>> ProvideRemoveGroupDTOFunc(int groupId)
        {
            return
                async c => await c.RecipientGroup
                    .Select(s => new AmendGroupDTO()
                    {
                        GroupId = s.RecipientGroupId,
                        GroupName = s.GroupName
                    }).SingleOrDefaultAsync(s => s.GroupId == groupId);
        }

        private async Task<Func<IJobQueueDataContext, Task<AmendRecipientDTO>>> ProvideSaveRecipentDTOFunc(string email)
        {
            return
                async c => await c.Recipient
                    .Select(s => new AmendRecipientDTO()
                    {
                        RecipientID = s.RecipientId,
                        EmailAddress = s.EmailAddress,
                        RecipientGroupIds = s.RecipientGroupRecipient.Select(recipientGroupRecipient => new EmailDistribution.Models.RecipientGroupRecipient()
                        {
                            RecipientGroupId = recipientGroupRecipient.RecipientGroupId,
                            RecipientId = recipientGroupRecipient.RecipientId
                        }).ToList()
                    }).SingleOrDefaultAsync(s => s.EmailAddress == email);
        }

        private async Task<Func<IJobQueueDataContext, Task<AmendRecipientDTO>>> ProvideRemoveRecipentDTOFunc(int recipientId)
        {
            return
                async c => await c.Recipient
                    .Select(s => new AmendRecipientDTO()
                    {
                        RecipientID = s.RecipientId,
                        EmailAddress = s.EmailAddress,
                        RecipientGroupIds = s.RecipientGroupRecipient.Select(recipientGroupRecipient => new EmailDistribution.Models.RecipientGroupRecipient()
                        {
                            RecipientGroupId = recipientGroupRecipient.RecipientGroupId,
                            RecipientId = recipientGroupRecipient.RecipientId
                        }).ToList()
                    }).SingleOrDefaultAsync(s => s.RecipientID == recipientId);
        }
    }
}
