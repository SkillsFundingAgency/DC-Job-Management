using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobScheduler.Interfaces.Models;

namespace ESFA.DC.JobScheduler.Interfaces
{
    public interface IMessageFactory
    {
        Task<MessageParameters> CreateMessageParametersAsync(string collectionName, long jobId);

        Task<MessageParameters> CreateMessageParametersAsync(string collectionName, long jobId, List<ITopicItem> topics, string topicName);
    }
}
