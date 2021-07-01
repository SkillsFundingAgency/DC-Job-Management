using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobScheduler.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.JobScheduler
{
    public class FailedJobNotificationService : IFailedJobNotificationService
    {
        private readonly IMessageFactory _messageFactory;
        private readonly IMessagingService _messagingService;
        private readonly ILogger _logger;
        private readonly IJobTopicTaskService _jobTopicTaskService;
        private readonly string SubscriptionName = "GenerateFM36Payments";
        private readonly string TaskName = "JobFailure";

        public FailedJobNotificationService(IMessageFactory messageFactory, IMessagingService messagingService, ILogger logger, IJobTopicTaskService jobTopicTaskService)
        {
            _messageFactory = messageFactory;
            _messagingService = messagingService;
            _logger = logger;
            _jobTopicTaskService = jobTopicTaskService;
        }

        public async Task SendMessageAsync(SubmittedJob job)
        {
            try
            {
                var tasks = new List<ITaskItem>()
                {
                    new TaskItem()
                    {
                        SupportsParallelExecution = false,
                        Tasks = new List<string>()
                        {
                            TaskName,
                        },
                    },
                };

                var topics = new List<ITopicItem>()
                {
                    new TopicItem(SubscriptionName, SubscriptionName, tasks),
                };

                var topicName = await _jobTopicTaskService.GetTopicName(job.CollectionId);

                var message = await _messageFactory.CreateMessageParametersAsync(job.CollectionName, job.JobId, topics, topicName);
                await _messagingService.SendMessageAsync(message);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured trying to send failed job notification", e);
            }
        }
    }
}
