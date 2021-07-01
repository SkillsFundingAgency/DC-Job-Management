using System.Collections.Generic;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.JobQueueManager.Model
{
    public sealed class TopicAndSubscription : ITopicItem
    {
        public string SubscriptionName { get; }

        public string SubscriptionSqlFilterValue { get; }

        public IReadOnlyList<ITaskItem> Tasks { get; }
    }
}
