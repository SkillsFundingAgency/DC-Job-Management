using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Ncs.Dss.Service.Dtos;
using ESFA.DC.Ncs.Dss.Service.Interfaces;
using ESFA.DC.Queueing.Interface;

namespace ESFA.DC.Ncs.Dss.Service
{
    public class DssService : IDssService<MessageCrossLoadFromNCSDto>
    {
        protected readonly ILogger _logger;
        private readonly IQueueSubscriptionService<MessageCrossLoadFromNCSDto> _queueSubscriptionService;
        private readonly IJobService _jobService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public DssService(
            IQueueSubscriptionService<MessageCrossLoadFromNCSDto> queueSubscriptionService,
            IJobService jobService,
            ILogger logger,
            IDateTimeProvider dateTimeProvider)
        {
            _queueSubscriptionService = queueSubscriptionService;
            _logger = logger;
            _jobService = jobService;
            _dateTimeProvider = dateTimeProvider;
        }

        public void Subscribe()
        {
            _queueSubscriptionService.Subscribe(ProcessNcsMessage, CancellationToken.None);
        }

        private async Task<IQueueCallbackResult> ProcessNcsMessage(MessageCrossLoadFromNCSDto message, IDictionary<string, object> messageProperties, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"ncs message received message with JobId: {message.JobId}, TouchpointId : {message.TouchpointId}");

                var collection = await _jobService.GetNcsCollectionByPeriodDate(message.Timestamp, cancellationToken);
                var periods = await _jobService.GetNcsCollectionPeriodsByCollection(collection.CollectionId, cancellationToken);
                var reportEndDate = DeriveReportEndDate(periods, message.Timestamp, out var currentPeriod);

                var ncsJob = new NcsJob()
                {
                    ExternalJobId = message.JobId.ToString(),
                    TouchpointId = message.TouchpointId,
                    ExternalTimestamp = message.Timestamp,
                    Ukprn = message.Ukprn,
                    DssContainer = message.ContainerName,
                    ReportFileName = message.ReportFilename,
                    CollectionName = collection.CollectionTitle,
                    ReportEndDate = reportEndDate,
                    DateTimeCreatedUtc = _dateTimeProvider.GetNowUtc(),
                    Period = currentPeriod,
                    Status = JobStatusType.Ready,
                    Priority = 1,
                    CreatedBy = "System"
                };

                var jobId = await _jobService.SubmitJob(ncsJob, cancellationToken);
                _logger.LogInfo($"Job:{jobId} sent to JobApi");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Application Exception occured.", ex);
                throw;
            }

            return new QueueCallbackResult(true, null);
        }

        private DateTime DeriveReportEndDate(IEnumerable<ReturnPeriod> returnPeriods, DateTime dateInMessage, out int currentPeriod)
        {
            var period = returnPeriods.SingleOrDefault(r => r.StartDateTimeUtc <= dateInMessage && r.EndDateTimeUtc >= dateInMessage);

            if (period == null)
            {
                period = returnPeriods
                    .OrderBy(x => x.StartDateTimeUtc)
                    .FirstOrDefault(o => o.StartDateTimeUtc > dateInMessage);
            }

            if (period == null)
            {
                throw new Exception($"We dont seem to have either current or future period defined for NCS covering date time : ${dateInMessage}");
            }

            currentPeriod = period.PeriodNumber;

            // get last day of month of calendar period in message.
            return new DateTime(period.CalendarYear, period.CalendarMonth, 1).AddMonths(1).AddDays(-1);
        }
    }
}
