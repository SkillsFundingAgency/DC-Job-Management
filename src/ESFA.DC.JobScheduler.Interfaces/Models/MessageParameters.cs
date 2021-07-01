using System.Collections.Generic;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.JobScheduler.Interfaces.Models
{
    public class MessageParameters
    {
        public MessageParameters(string collectionName)
        {
            CollectionName = collectionName;
            JobContextMessage = new JobContextMessage();
        }

        public IJobContextMessage JobContextMessage { get; set; }

        public string SubscriptionLabel { get; set; }

        public IDictionary<string, object> TopicParameters { get; set; }

        public string CollectionName { get; set; }

        public string TopicName { get; set; }
    }
}
