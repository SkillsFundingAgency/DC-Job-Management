using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.ExternalData;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.JobScheduler.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.JobScheduler
{
    public class JobQueueHandler : IJobQueueHandler
    {
        private readonly IJobSchedulerStatusManager _jobSchedulerStatusManager;
        private readonly IMessageFactory _jobContextMessageFactory;
        private readonly IMessagingService _messagingService;
        private readonly IJobManager _jobQueueManager;
        private readonly IAuditor _auditor;
        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IExternalDataScheduleService _externalDataScheduleService;
        private readonly IServiceBusMessageLogger _serviceBusMessageLogger;

        public JobQueueHandler(
            IMessagingService messagingService,
            IJobManager jobQueueManager,
            IAuditor auditor,
            IJobSchedulerStatusManager jobSchedulerStatusManager,
            IMessageFactory jobContextMessageFactory,
            ILogger logger,
            IDateTimeProvider dateTimeProvider,
            IExternalDataScheduleService externalDataScheduleService,
            IServiceBusMessageLogger serviceBusMessageLogger)
        {
            _jobSchedulerStatusManager = jobSchedulerStatusManager;
            _jobContextMessageFactory = jobContextMessageFactory;
            _jobQueueManager = jobQueueManager;
            _messagingService = messagingService;
            _auditor = auditor;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _externalDataScheduleService = externalDataScheduleService;
            _serviceBusMessageLogger = serviceBusMessageLogger;
        }

        public async Task ProcessNextJobAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                while (true)
                {
                    try
                    {
                        if (await _jobSchedulerStatusManager.IsJobQueueProcessingEnabledAsync())
                        {
                            await QueueAnyNewReferenceDataJobsAsync(cancellationToken);

                            await ProcessAnyNewJobsAsync(cancellationToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error occured in job scheduler - will continue to pick new jobs", ex);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured in job scheduler - Scheduler will stop looking for new jobs", ex);
            }
        }

        public async Task MoveJobForProcessingAsync(Job job)
        {
            if (job == null)
            {
                return;
            }

            _logger.LogInfo($"Job id: {job.JobId} received for moving to queue", jobIdOverride: job.JobId);

            var message = await _jobContextMessageFactory.CreateMessageParametersAsync(job.CollectionName, job.JobId);

            try
            {
                var jobStatusUpdated = await _jobQueueManager.UpdateJobStatus(job.JobId, JobStatusType.MovedForProcessing);

                _logger.LogInfo($"Job id: {job.JobId} status updated successfully", jobIdOverride: job.JobId);

                if (jobStatusUpdated)
                {
                    try
                    {
                        await _messagingService.SendMessageAsync(message);
                        await _auditor.AuditAsync(message.JobContextMessage, AuditEventType.JobSubmitted);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Job id: {job.JobId} sending to service bus failed", ex, jobIdOverride: job.JobId);
                        await _auditor.AuditAsync(
                            message.JobContextMessage,
                            AuditEventType.ServiceFailed,
                            $"Failed to send message to Service bus queue with exception : {ex}");

                        await _jobQueueManager.UpdateJobStatus(job.JobId, JobStatusType.Failed);
                    }
                }
                else
                {
                    _logger.LogWarning($"Job id : {job.JobId} failed to send to service bus", jobIdOverride: job.JobId);
                    await _auditor.AuditAsync(
                        message.JobContextMessage,
                        AuditEventType.JobFailed,
                        "Failed to update job status, no message is added to the service bus queue");
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Job id: {job.JobId}", exception, jobIdOverride: job.JobId);
                await _auditor.AuditAsync(
                    message.JobContextMessage,
                    AuditEventType.ServiceFailed,
                    $"Failed to update job status with exception : {exception}");
            }
        }

        private async Task QueueAnyNewReferenceDataJobsAsync(CancellationToken cancellationToken)
        {
            IEnumerable<string> collections = await _externalDataScheduleService.GetJobs(true, cancellationToken);
            foreach (var collection in collections)
            {
                Job refDataJob = new Job
                {
                    DateTimeCreatedUtc = _dateTimeProvider.GetNowUtc(),
                    CollectionName = collection,
                    Priority = 1,
                    Status = JobStatusType.Ready,
                    CreatedBy = "System"
                };

                /* long id = */
                await _jobQueueManager.AddJob(refDataJob);
            }
        }

        private async Task ProcessAnyNewJobsAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Job> jobs = await _jobQueueManager.GetJobsByPriorityAsync(25);

            foreach (Job job in jobs)
            {
                _logger.LogInfo($"Got job id: {job.JobId}", jobIdOverride: job.JobId);
                await MoveJobForProcessingAsync(job);
            }
        }
    }
}