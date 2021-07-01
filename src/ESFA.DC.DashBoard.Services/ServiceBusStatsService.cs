using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DashBoard.Interface;
using ESFA.DC.DashBoard.Models.ServiceBus;
using ESFA.DC.DashBoard.Models.Settings;
using ESFA.DC.Logging.Interfaces;
using Microsoft.Azure.ServiceBus.Management;

namespace ESFA.DC.DashBoard.Services
{
    public sealed class ServiceBusStatsService : IServiceBusStatsService
    {
        private const string _ilrPrefix = "ilr";
        private readonly string[] _ignoredQueues = { "crossloadin", "crossloadout" };
        private readonly string[] _replacedQueues = { "queue" };
        private readonly string[] _ignoredTopics = { "bundle-1" };
        private readonly string[] _replacedTopics = { "submission", "topic" };
        private readonly string[] _replacedSubscriptions = { "GenerateFM36" };
        private readonly Tuple<string, string>[] _changedSubscriptions = { new Tuple<string, string>("ReferenceDataRetrieval", "Ref Data") };

        private readonly ServiceBusSettings _serviceBusSettings;
        private readonly ILogger _logger;

        public ServiceBusStatsService(ServiceBusSettings serviceBusSettings, ILogger logger)
        {
            _serviceBusSettings = serviceBusSettings;
            _logger = logger;
        }

        public async Task<ServiceBusStatusModel> ProvideAsync(CancellationToken cancellationToken)
        {
            List<ServiceBusEntityModel> queuesList = new List<ServiceBusEntityModel>();
            List<ServiceBusEntityModel> topicsList = new List<ServiceBusEntityModel>();
            List<ServiceBusEntityModel> topicsIlr = new List<ServiceBusEntityModel>();

            try
            {
                var mgmtClient = new ManagementClient(_serviceBusSettings.ServiceBusManagementConnectionString);

                var queues = await mgmtClient.GetQueuesAsync(cancellationToken: cancellationToken);

                foreach (QueueDescription queueDescription in queues)
                {
                    if (_ignoredQueues.Contains(queueDescription.Path, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var runtimeInfo = await mgmtClient.GetQueueRuntimeInfoAsync(queueDescription.Path, cancellationToken);

                    queuesList.Add(new ServiceBusEntityModel
                    {
                        Name = FormatName(queueDescription.Path, _replacedQueues),
                        DeadLetterMessageCount = runtimeInfo.MessageCountDetails.DeadLetterMessageCount,
                        MessageCount = runtimeInfo.MessageCountDetails.ActiveMessageCount,
                        TransferCount = runtimeInfo.MessageCountDetails.TransferMessageCount
                    });
                }

                var topics = await mgmtClient.GetTopicsAsync(cancellationToken: cancellationToken);

                foreach (TopicDescription topicDescription in topics)
                {
                    if (_ignoredTopics.Contains(topicDescription.Path, StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var runtimeInfo =
                        await mgmtClient.GetTopicRuntimeInfoAsync(topicDescription.Path, cancellationToken);

                    var model = new ServiceBusEntityModel
                    {
                        Name = FormatName(topicDescription.Path, _replacedTopics),
                        DeadLetterMessageCount = runtimeInfo.MessageCountDetails.DeadLetterMessageCount,
                        MessageCount = runtimeInfo.MessageCountDetails.ActiveMessageCount,
                        TransferCount = runtimeInfo.MessageCountDetails.TransferMessageCount
                    };

                    var subscriptions = await mgmtClient.GetSubscriptionsAsync(topicDescription.Path, cancellationToken: cancellationToken);
                    foreach (SubscriptionDescription subscriptionDescription in subscriptions)
                    {
                        var runtimeInfoSubscription =
                            await mgmtClient.GetSubscriptionRuntimeInfoAsync(runtimeInfo.Path, subscriptionDescription.SubscriptionName, cancellationToken);

                        model.MessageCount += runtimeInfoSubscription.MessageCount;

                        if (topicDescription.Path.StartsWith(_ilrPrefix, StringComparison.OrdinalIgnoreCase))
                        {
                            string name = FormatName(subscriptionDescription.SubscriptionName, _replacedSubscriptions, _changedSubscriptions);
                            ServiceBusEntityModel topic = topicsIlr.SingleOrDefault(x => x.Name == name);

                            if (topic != null)
                            {
                                topic.MessageCount += runtimeInfoSubscription.MessageCount;
                                continue;
                            }

                            topicsIlr.Add(new ServiceBusEntityModel
                            {
                                DeadLetterMessageCount = 0,
                                MessageCount = runtimeInfoSubscription.MessageCount,
                                Name = name,
                                TransferCount = 0
                            });
                        }
                    }

                    topicsList.Add(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get Service Bus stats", ex);
            }

            return new ServiceBusStatusModel
            {
                Queues = queuesList,
                Topics = topicsList,
                Ilr = topicsIlr
            };
        }

        private string FormatName(string name, IEnumerable<string> tokensToDelete, IEnumerable<Tuple<string, string>> tokensToChange = null)
        {
            foreach (string token in tokensToDelete)
            {
                name = name.Replace(token, string.Empty);
            }

            if (tokensToChange != null)
            {
                foreach (Tuple<string, string> change in tokensToChange)
                {
                    name = name.Replace(change.Item1, change.Item2);
                }
            }

            return name;
        }
    }
}