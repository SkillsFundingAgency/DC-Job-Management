using System;
using ESFA.DC.Queueing.Interface.Configuration;
using Newtonsoft.Json;

namespace ESFA.DC.Job.WebApi.Settings
{
    public sealed class ServiceBusTopicConfiguration : ITopicConfiguration
    {
        [JsonRequired]
        public string ConnectionString { get; set; }

        public string SubscriptionName { get; set; }

        public string TopicName { get; set; }

        public int MaxConcurrentCalls => 1;

        public int MinimumBackoffSeconds => 2;

        public int MaximumBackoffSeconds => 5;

        public int MaximumRetryCount => 3;

        public TimeSpan MaximumCallbackTimeSpan => new TimeSpan(0, 10, 0);
    }
}
