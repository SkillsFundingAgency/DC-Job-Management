using System;
using ESFA.DC.Queueing.Interface.Configuration;
using Newtonsoft.Json;

namespace ESFA.DC.JobStatus.Service.Configuration
{
    public sealed class JobStatusQueueConfiguration : IQueueConfiguration
    {
        public JobStatusQueueConfiguration(string connectionString, string queueName)
        {
            ConnectionString = connectionString;
            QueueName = queueName;
        }

        [JsonRequired]
        public string ConnectionString { get; set; }

        [JsonRequired]
        public string QueueName { get; set; }

        public string TopicName => string.Empty;

        public int MaxConcurrentCalls => 1;

        public int MinimumBackoffSeconds => 2;

        public int MaximumBackoffSeconds => 5;

        public int MaximumRetryCount => 3;

        public TimeSpan MaximumCallbackTimeSpan => new TimeSpan(0, 10, 0);
    }
}
