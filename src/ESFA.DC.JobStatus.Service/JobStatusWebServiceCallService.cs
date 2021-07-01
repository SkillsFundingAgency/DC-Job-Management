using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Telemetry.Interfaces;

namespace ESFA.DC.JobStatus.Service
{
    public sealed class JobStatusWebServiceCallService<T> : BaseWebServiceCallService, IJobStatusWebServiceCallService<T>
        where T : JobStatusDto, new()
    {
        private readonly IQueueSubscriptionService<T> _queueSubscriptionService;

        public JobStatusWebServiceCallService(
            IJobStatusWebServiceCallServiceConfig jobStatusWebServiceCallServiceConfig,
            IQueueSubscriptionService<T> queueSubscriptionService,
            ISerializationService serializationService,
            ILogger logger,
            ITelemetry telemetry)
            : base(jobStatusWebServiceCallServiceConfig, serializationService, logger, telemetry)
        {
            _queueSubscriptionService = queueSubscriptionService;
        }

        public void Subscribe()
        {
            _queueSubscriptionService.Subscribe((dto, props, token) => ProcessMessageAsync(dto, token), CancellationToken.None);
        }

        private async Task<IQueueCallbackResult> ProcessMessageAsync(T jobStatusDto, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"message received by Job status service to update status : {jobStatusDto.JobStatus} for jobId : {jobStatusDto.JobId}", jobIdOverride: jobStatusDto.JobId);
                var response = await SendStatusAsync(jobStatusDto.JobId, jobStatusDto.JobStatus, cancellationToken, jobStatusDto.NumberOfLearners);

                return new QueueCallbackResult(response.IsSuccessStatusCode, null);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to post job status message", ex);
                return new QueueCallbackResult(false, ex);
            }
        }
    }
}
