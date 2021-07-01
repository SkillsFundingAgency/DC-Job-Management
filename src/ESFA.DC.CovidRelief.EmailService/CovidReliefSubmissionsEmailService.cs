using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CovidRelief.EmailService.Interfaces;
using ESFA.DC.CovidRelief.Services.Interfaces;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.CovidRelief.EmailService
{
    public class CovidReliefSubmissionsEmailService : ICovidReliefSubmissionsEmailService
    {
        private readonly IEmailNotifier _emailNotifier;
        private readonly ICollectionService _collectionService;
        private readonly ICovidReliefSubmissionService _covidReliefSubmissionService;
        private readonly ICovidReliefEmailAddressesService _covidReliefEmailAddressesService;
        private readonly ICovidReliefEmailService _covidReliefEmailService;
        private readonly ICollectionEmailTemplateManager _collectionEmailTemplateManager;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public CovidReliefSubmissionsEmailService(
            IEmailNotifier emailNotifier,
            ICollectionService collectionService,
            ICovidReliefSubmissionService covidReliefSubmissionService,
            ICovidReliefEmailAddressesService covidReliefEmailAddressesService,
            ICovidReliefEmailService covidReliefEmailService,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
        {
            _emailNotifier = emailNotifier;
            _collectionService = collectionService;
            _covidReliefSubmissionService = covidReliefSubmissionService;
            _covidReliefEmailAddressesService = covidReliefEmailAddressesService;
            _covidReliefEmailService = covidReliefEmailService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task Execute()
        {
            _logger.LogInfo("Covid Relief Submissions Email Service Started.");

            await SendEmails(CollectionTypeConstants.Covid);
            await SendEmails(CollectionTypeConstants.Covid2);
        }

        private async Task SendEmails(string collectionType)
        {
            var currentDateTime = _dateTimeProvider.GetNowUtc();

            var collection = await _collectionService.GetCollectionByDateAsync(CancellationToken.None, collectionType, currentDateTime);

            if (collection?.IsOpen == false)
            {
                _logger.LogDebug($"Covid relief email Collection type : {collectionType}, no open period. No need to send the email");
                return;
            }

            var emails = _covidReliefEmailAddressesService.GetCovidReliefEmailAddresses(collectionType);

            if (!emails.Any())
            {
                _logger.LogDebug("No emails to send, as there are no emails found to be sent.");
                return;
            }

            var submissionDates = await _covidReliefSubmissionService.GetSubmissionsByDate(collectionType, _dateTimeProvider.GetNowUtc());

            if (!submissionDates.Any())
            {
                _logger.LogInfo("No covid relief submissions found.");
                return;
            }

            var numberOfEmailsSent = await _covidReliefEmailService.SendSubmissionNotifications(collection.CollectionId, emails, submissionDates);

            _logger.LogInfo($"Covid Relief Submissions Email Service Stopped.{Environment.NewLine}Number of emails sent: {numberOfEmailsSent}");
        }
    }
}
