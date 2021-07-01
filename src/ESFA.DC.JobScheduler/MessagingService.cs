using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.JobContext;
using ESFA.DC.JobScheduler.Interfaces;
using ESFA.DC.JobScheduler.Interfaces.Models;
using ESFA.DC.JobScheduler.Settings;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Queueing.Interface.Configuration;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.JobScheduler
{
    public class MessagingService : IMessagingService
    {
        private readonly Dictionary<string, ITopicPublishService<JobContextDto>> _topicPublishServices = new Dictionary<string, ITopicPublishService<JobContextDto>>();
        private readonly JobContextMapper _jobContextMapper;
        private readonly ITopicConfiguration _topicConfiguration;
        private readonly IComponentContext _componentContext;
        private readonly IServiceBusMessageLogger _serviceBusMessageLogger;
        private readonly ILogger _logger;

        public MessagingService(
            JobContextMapper jobContextMapper,
            ITopicConfiguration topicConfiguration,
            IComponentContext componentContext,
            IServiceBusMessageLogger serviceBusMessageLogger,
            ILogger logger)
        {
            _jobContextMapper = jobContextMapper;
            _topicConfiguration = topicConfiguration;
            _componentContext = componentContext;
            _serviceBusMessageLogger = serviceBusMessageLogger;
            _logger = logger;
        }

        public async Task SendMessageAsync(MessageParameters messageParameters)
        {
            ITopicPublishService<JobContextDto> topicPublishService;

            if (_topicPublishServices.ContainsKey(messageParameters.CollectionName))
            {
                topicPublishService = _topicPublishServices[messageParameters.CollectionName];
            }
            else
            {
                topicPublishService = new TopicPublishService<JobContextDto>(
                    new ServiceBusTopicConfiguration()
                    {
                        ConnectionString = _topicConfiguration.ConnectionString,
                        TopicName = messageParameters.TopicName,
                    },
                    _componentContext.Resolve<IJsonSerializationService>());

                _topicPublishServices.Add(messageParameters.CollectionName, topicPublishService);
            }

            JobContextMessage jobContextMessage = (JobContextMessage)messageParameters.JobContextMessage;

            await topicPublishService.PublishAsync(
                _jobContextMapper.MapFrom(jobContextMessage),
                messageParameters.TopicParameters,
                messageParameters.SubscriptionLabel);

            try
            {
                _logger.LogInfo($"Logging message to the DB for job id : {jobContextMessage.JobId}", null, jobContextMessage.JobId);
                await _serviceBusMessageLogger.LogMessageAsync(jobContextMessage, System.Threading.CancellationToken.None);
            }
            catch (Exception e)
            {
                _logger.LogError($"Logging message to the DB for job id : {jobContextMessage.JobId}", e);
            }
        }
    }
}