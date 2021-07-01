using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;

namespace ESFA.DC.PeriodEnd.Services
{
    public class EmailDistributionService : IEmailDistributionService
    {
        private readonly ILogger _logger;
        private readonly IEmailDistributionRepository _emailDistributionRepository;

        public EmailDistributionService(ILogger logger, IEmailDistributionRepository emailDistributionRepository)
        {
            _logger = logger;
            _emailDistributionRepository = emailDistributionRepository;
        }

        public async Task<IEnumerable<Recipient>> GetEmailDistributionGroupRecipients(int recipientGroupId)
        {
            return await _emailDistributionRepository.GetEmailDistributionGroupRecipients(recipientGroupId);
        }

        public async Task<IEnumerable<RecipientGroup>> GetEmailDistributionGroups()
        {
            return await _emailDistributionRepository.GetEmailDistributionGroups();
        }

        public async Task<EmailTemplate> GetEmailTemplate(int emailId)
        {
            return await _emailDistributionRepository.GetTemplate(emailId);
        }

        public async Task<IEnumerable<EmailTemplate>> GetEmailTemplates()
        {
            return await _emailDistributionRepository.GetEmailTemplates();
        }

        public async Task<bool> RemoveRecipient(int recipientId, int recipientGroupId)
        {
            return await _emailDistributionRepository.RemoveRecipient(recipientId, recipientGroupId);
        }

        public async Task<bool> RemoveRecipientGroup(int recipientGroupId)
        {
            return await _emailDistributionRepository.RemoveGroup(recipientGroupId);
        }

        public async Task<bool> SaveRecipientGroup(string groupName)
        {
            return await _emailDistributionRepository.SaveGroup(groupName);
        }

        public async Task<SaveRecipientResponse> SaveRecipientAsync(Recipient recipient, CancellationToken cancellationToken)
        {
            var existingGroupsList = await _emailDistributionRepository.GetEmailDistributionGroupsAsync(recipient.EmailAddress, cancellationToken);

            var associatedGroups = existingGroupsList.Where(x => recipient.RecipientGroupIds.Contains(x.RecipientGroupId)).ToList();
            var nonAssociatedGroups = recipient.RecipientGroupIds.Where(x => !existingGroupsList.Select(y => y.RecipientGroupId).Contains(x));

            // Save only the non Associated Groups
            recipient.RecipientGroupIds = nonAssociatedGroups.ToList();
            await _emailDistributionRepository.SaveRecipientAsync(recipient, cancellationToken);

            return associatedGroups.Any() ? new SaveRecipientResponse() { IsDuplicateEmail = true, RecipientGroups = associatedGroups } : new SaveRecipientResponse() { IsDuplicateEmail = false };
        }

        public async Task<bool> SaveTemplate(EmailTemplate emailTemplate)
        {
            return await _emailDistributionRepository.SaveTemplate(emailTemplate);
        }

        public async Task<RecipientGroup> GetEmailDistributionGroup(int recipientGroupId)
        {
            return await _emailDistributionRepository.GetEmailDistributionGroup(recipientGroupId);
        }
    }
}
