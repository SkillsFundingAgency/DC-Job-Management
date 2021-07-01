using System;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.PeriodEnd.DataAccess
{
    public class CommsRepository : ICommsRepository
    {
        private readonly ILogger _logger;
        private readonly Func<IJobQueueDataContext> _contextFactory;

        public CommsRepository(
            ILogger logger,
            Func<IJobQueueDataContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task<PathItemEmailDetails> GetEmailDetails(int hubEmailId)
        {
            PathItemEmailDetails emailDetails;

            try
            {
                using (var context = _contextFactory())
                {
                    emailDetails = await context.Email
                        .Where(c => c.HubEmailId == hubEmailId)
                        .Select(c => new PathItemEmailDetails
                        {
                            TemplateId = c.TemplateId,
                            Recipients = c.EmailRecipientGroup
                                .SelectMany(cr => cr.RecipientGroup.RecipientGroupRecipient.Select(x => x.Recipient.EmailAddress))
                                .ToList()
                        }).SingleAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }

            return emailDetails;
        }
    }
}