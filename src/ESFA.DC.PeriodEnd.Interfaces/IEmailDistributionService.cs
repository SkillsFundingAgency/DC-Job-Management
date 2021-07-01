using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IEmailDistributionService
    {
        Task<IEnumerable<RecipientGroup>> GetEmailDistributionGroups();

        Task<bool> SaveRecipientGroup(string groupName);

        Task<SaveRecipientResponse> SaveRecipientAsync(Recipient recipient, CancellationToken cancellationToken);

        Task<bool> SaveTemplate(EmailTemplate emailTemplate);

        Task<IEnumerable<EmailTemplate>> GetEmailTemplates();

        Task<EmailTemplate> GetEmailTemplate(int emailId);

        Task<IEnumerable<Recipient>> GetEmailDistributionGroupRecipients(int recipientGroupId);

        Task<bool> RemoveRecipient(int recipientId, int recipientGroupId);

        Task<bool> RemoveRecipientGroup(int recipientGroupId);

        Task<RecipientGroup> GetEmailDistributionGroup(int recipientGroupId);
    }
}
