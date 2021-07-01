using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;

namespace ESFA.DC.PeriodEnd.Interfaces.DataAccess
{
    public interface IEmailDistributionRepository
    {
        Task<IEnumerable<RecipientGroup>> GetEmailDistributionGroups();

        Task<IList<RecipientGroup>> GetEmailDistributionGroupsAsync(string recipientEmailAddress, CancellationToken cancellationToken);

        Task<bool> SaveRecipientAsync(Recipient recipient, CancellationToken cancellationToken);

        Task<IEnumerable<Recipient>> GetEmailDistributionGroupRecipients(int recipientGroupId);

        Task<bool> RemoveRecipient(int recipientId, int recipientGroupId);

        Task<bool> SaveGroup(string groupName);

        Task<bool> RemoveGroup(int recipientGroupId);

        Task<IEnumerable<EmailTemplate>> GetEmailTemplates();

        Task<bool> SaveTemplate(EmailTemplate emailTemplate);

        Task<EmailTemplate> GetTemplate(int emailId);

        Task<RecipientGroup> GetEmailDistributionGroup(int recipientGroupId);
    }
}
