using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContext;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobStatus;
using ESFA.DC.JobStatus.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Telemetry.Interfaces;

namespace ESFA.DC.FailedJob.Service
{
    public sealed class FailedJobsWebServiceCallService : BaseWebServiceCallService, IJobStatusWebServiceCallService<JobContextDto>
    {
        private readonly IQueueSubscriptionService<JobContextDto> _queueSubscriptionService;
        private readonly string DASSubscriptionName = "GenerateFM36Payments";
        private readonly string DedsSubscriptionName = "Deds";
        private readonly string[] DASTaskNames = { "Process", "ProcessPeriodEnd" };

        public FailedJobsWebServiceCallService(
            IJobStatusWebServiceCallServiceConfig jobStatusWebServiceCallServiceConfig,
            IQueueSubscriptionService<JobContextDto> queueSubscriptionService,
            ISerializationService serializationService,
            ILogger logger,
            ITelemetry telemetry)
            : base(jobStatusWebServiceCallServiceConfig, serializationService, logger, telemetry)
        {
            _queueSubscriptionService = queueSubscriptionService;
        }

        public void Subscribe()
        {
            _queueSubscriptionService.Subscribe(ProcessDeadLetterMessageAsync, CancellationToken.None);
        }

        private async Task<IQueueCallbackResult> ProcessDeadLetterMessageAsync(JobContextDto jobContextDto, IDictionary<string, object> messageProperties, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogError($"failed job service received message with topic pointer: {jobContextDto.TopicPointer}, job id : {jobContextDto.JobId}", jobIdOverride: jobContextDto.JobId);

                bool irrecoverable = false;
                if (messageProperties.TryGetValue("Exceptions", out object exceptions))
                {
                    string[] exceptionList = exceptions.ToString().Split(':');
                    if (exceptionList.Contains("NullReferenceException"))
                    {
                        irrecoverable = true;
                    }
                }

                var sendFailureToDas = IsDedsStep(jobContextDto) || IsFm36Step(jobContextDto);

                _logger.LogDebug($"failed job service, send to das value : {sendFailureToDas}, irrecoverable : {irrecoverable} ", jobIdOverride: jobContextDto.JobId);

                HttpResponseMessage response;

                if (irrecoverable)
                {
                    response = await SendStatusAsync(jobContextDto.JobId, (int)JobStatusType.Failed, cancellationToken, -1, sendFailureToDas);
                }
                else
                {
                    response = await SendStatusAsync(jobContextDto.JobId, (int)JobStatusType.FailedRetry, cancellationToken, -1, sendFailureToDas);
                }

                _logger.LogDebug($"failed job service, sent message to status api result : {response.IsSuccessStatusCode}", jobIdOverride: jobContextDto.JobId);

                return new QueueCallbackResult(response.IsSuccessStatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to post dead letter job status message", ex, jobIdOverride: jobContextDto.JobId);
                return new QueueCallbackResult(false, ex);
            }
        }

        private bool IsFm36Step(JobContextDto jobContextDto)
        {
            return jobContextDto.Topics[jobContextDto.TopicPointer].SubscriptionName.Equals(DASSubscriptionName, StringComparison.OrdinalIgnoreCase)
                   && jobContextDto.Topics[jobContextDto.TopicPointer].Tasks.Any(task => task.Tasks.Any(taskName => DASTaskNames.Any(dasTaskName => string.Equals(taskName, dasTaskName, StringComparison.OrdinalIgnoreCase))));
        }

        private bool IsDedsStep(JobContextDto jobContextDto)
        {
            return jobContextDto.Topics[jobContextDto.TopicPointer].SubscriptionName.Equals(DedsSubscriptionName, StringComparison.OrdinalIgnoreCase)
                   && jobContextDto.Topics.Any(x => x.SubscriptionName.Equals(DASSubscriptionName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
