using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.CovidRelief.Models;
using ESFA.DC.CovidRelief.Services.Interfaces;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.CovidRelief.Services
{
    public class CovidReliefEmailService : ICovidReliefEmailService
    {
        private readonly IEmailNotifier _emailNotifier;
        private readonly ICollectionEmailTemplateManager _collectionEmailTemplateManager;
        private readonly ICollectionService _collectionService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public CovidReliefEmailService(
            IEmailNotifier emailNotifier,
            ICollectionEmailTemplateManager collectionEmailTemplateManager,
            ICollectionService collectionService,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
        {
            _emailNotifier = emailNotifier;
            _collectionEmailTemplateManager = collectionEmailTemplateManager;
            _collectionService = collectionService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task<int> SendSubmissionNotifications(int collectionId, IEnumerable<string> emails, IEnumerable<SubmissionDate> submissionsByDate)
        {
            StringBuilder submissionDateStringBuilder = new StringBuilder();
            const string dateFormatString = "d MMMM HH:mm:ss";
            int emailsSentCount = 0;

            var emailTemplate = await _collectionEmailTemplateManager.GetTemplate(collectionId, JobStatusType.Completed);

            if (emailTemplate == null)
            {
                _logger.LogError("No valid Active Email Template for Collection");
                return 0;
            }

            foreach (var submission in submissionsByDate)
            {
                submissionDateStringBuilder.Append(submission.Ukprn + ", " + _dateTimeProvider.ConvertUtcToUk(submission.DateTimeSubmittedUtc).ToString(dateFormatString) + Environment.NewLine);
            }

            foreach (var email in emails)
            {
                _logger.LogInfo($"Sending Covid relief email to {email}");

                var parameters = new Dictionary<string, dynamic>
                {
                    { "SubmissionsData", submissionDateStringBuilder.ToString() }
                };

                try
                {
                    await _emailNotifier.SendEmail(email, emailTemplate, parameters);
                    emailsSentCount++;
                }
                catch (Notify.Exceptions.NotifyClientException e)
                {
                    _logger.LogError($"Error sending email to email address: {email}", e);
                }
            }

            return emailsSentCount;
        }

        public async Task SendEmail(string email, int collectionId)
        {
            try
            {
                var template = await _collectionEmailTemplateManager.GetTemplate(collectionId, JobStatusType.Ready);

                if (template != null)
                {
                    await _emailNotifier.SendEmail(email, template, null);
                    _logger.LogInfo($"Email for provider relief submission sent to email : {email}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while sending email email to provider relief submission, email : {email}", ex);
            }
        }
    }
}
