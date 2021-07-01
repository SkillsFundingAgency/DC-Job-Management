using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DeadLetter.Service.Configuration;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Queueing.Interface.Configuration;
using ESFA.DC.Serialization.Interfaces;
using Notify.Client;

namespace ESFA.DC.DeadLetter.Service
{
    public sealed class DeadLetterRetryService<T>
        where T : new()
    {
        private const string DeadLetterEmailTemplateId = "2bb43090-18f0-4c04-b670-577827c3af65";

        private readonly IQueueConfiguration _queueConfiguration;
        private readonly IQueueSubscriptionService<string> _queueSubscriptionService;
        private readonly IQueuePublishService<T> _queuePublishService;
        private readonly EmailConfig _emailConfig;
        private readonly ISerializationService _jsonSerializationService;
        private readonly ILogger _logger;

        public DeadLetterRetryService(
            IQueueConfiguration queueConfiguration,
            EmailConfig emailConfig,
            ISerializationService stringSerializationService,
            ISerializationService jsonSerializationService,
            ILogger logger)
        {
            _queueConfiguration = queueConfiguration;
            _emailConfig = emailConfig;
            _jsonSerializationService = jsonSerializationService;
            _logger = logger;

            var receiveConfiguration = new DeadLetterQueueConfiguration(queueConfiguration.ConnectionString, queueConfiguration.QueueName + "/$DeadLetterQueue", queueConfiguration.MaxConcurrentCalls);
            _queueSubscriptionService = new QueueSubscriptionService<string>(receiveConfiguration, stringSerializationService, logger);

            var publishConfiguration = new DeadLetterQueueConfiguration(queueConfiguration.ConnectionString, queueConfiguration.QueueName, queueConfiguration.MaxConcurrentCalls);
            _queuePublishService = new QueuePublishService<T>(publishConfiguration, jsonSerializationService);

            _queueSubscriptionService.Subscribe(ProcessDeadLetterMessageAsync, CancellationToken.None);

            logger.LogInfo($"Started monitoring for dead letter messages on queue {queueConfiguration.QueueName}");
        }

        private async Task<IQueueCallbackResult> ProcessDeadLetterMessageAsync(string messageBody, IDictionary<string, object> messageProperties, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogWarning($"Dead letter service on {_queueConfiguration.QueueName} received message with body {messageBody}");

                // Can we de-serialise the message?
                T messageObject = default(T);
                try
                {
                    messageObject = _jsonSerializationService.Deserialize<T>(messageBody);
                }
                catch (Exception e)
                {
                    _logger.LogWarning($"Dead letter service on {_queueConfiguration.QueueName} received message with invalid body json {messageBody}");
                    messageProperties.Add("Status", "Invalid JSON");
                    SendEmail(messageBody, messageProperties);

                    return new QueueCallbackResult(true, null);
                }

                // Check if this has a tracking ID - IE Has it already been through the dead letter retry process?
                if (messageProperties.TryGetValue(Queueing.Constants.MessageProperties.TrackingId, out object trackingId))
                {
                    _logger.LogWarning($"Dead letter service on {_queueConfiguration.QueueName} received message with Tracking Id {trackingId} - Emailing notification");
                    SendEmail(messageBody, messageProperties);

                    return new QueueCallbackResult(true, null);
                }

                // This message is new to the dead letter Q, send it round again, but with a Tracking Id attached
                var newTrackingId = Guid.NewGuid();
                await _queuePublishService.PublishWithTrackingIdAsync(messageObject, newTrackingId);

                _logger.LogWarning($"Dead letter service on {_queueConfiguration.QueueName} re-trying message adding Tracking Id {newTrackingId} to body {messageBody}");
                return new QueueCallbackResult(true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Dead letter service on {_queueConfiguration.QueueName} failed to process dead letter message {messageBody}", ex);
                return new QueueCallbackResult(false, ex);
            }
        }

        private void SendEmail(string messageBody, IDictionary<string, object> messageProperties)
        {
            if (string.IsNullOrEmpty(_emailConfig.ApiKey))
            {
                _logger.LogError("Email Api key is empty");
                return;
            }

            // Build the message details to be displayed from the template
            var messageDetails = string.Empty;
            foreach (var messageProperty in messageProperties)
            {
                messageDetails +=
                    $"{messageProperty.Key} - {messageProperty.Value}" + Environment.NewLine;
            }

            var emailParams = new Dictionary<string, dynamic>
            {
                { "QueueName", _queueConfiguration.QueueName },
                { "MessageBody", messageBody },
                { "MessageDetails", messageDetails },
            };

            var client = new NotificationClient(_emailConfig.ApiKey);
            foreach (var recipient in _emailConfig.Recipients)
            {
                client.SendEmail(recipient, DeadLetterEmailTemplateId, emailParams);
            }
        }
    }
}
