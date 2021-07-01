using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Audit.Models.DTOs.PeriodEnd;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.Email;

namespace ESFA.DC.PeriodEnd.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;
        private readonly ICommsRepository _commsRepository;
        private readonly IEmailNotifier _emailNotifier;
        private readonly IAuditFactory _auditFactory;

        public EmailService(
            ILogger logger,
            ICommsRepository commsRepository,
            IEmailNotifier emailNotifier,
            IAuditFactory auditFactory)
        {
            _logger = logger;
            _commsRepository = commsRepository;
            _emailNotifier = emailNotifier;
            _auditFactory = auditFactory;
        }

        public async Task SendEmail(int hubEmailId, Dictionary<string, dynamic> emailParameters)
        {
            _logger.LogDebug($"In EmailService SendEmail with emailId {hubEmailId}");

            var emailDetails = await _commsRepository.GetEmailDetails(hubEmailId);

            if (emailDetails?.Recipients == null)
            {
                var errorMessage = $"No email details found for emailId {hubEmailId}";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            foreach (var recipient in emailDetails.Recipients)
            {
                try
                {
                    Func<SendEmailDTO> func = () => ProvideSendEmailDTO(hubEmailId);
                    var audit = _auditFactory.BuildRequestAudit<SendEmailDTO>(func);
                    await _emailNotifier.SendEmail(recipient, emailDetails.TemplateId, emailParameters);
                    await audit.AfterAndSaveAsync(CancellationToken.None);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    throw;
                }
            }
        }

        private SendEmailDTO ProvideSendEmailDTO(int hubItemID)
        {
            return new SendEmailDTO() { HubEmailID = hubItemID };
        }
    }
}