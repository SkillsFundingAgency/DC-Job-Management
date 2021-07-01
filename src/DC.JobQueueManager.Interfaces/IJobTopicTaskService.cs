using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IJobTopicTaskService
    {
        Task<IEnumerable<ITopicItem>> GetTopicItems(int colletionId, bool isFirstStage = false, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetTopicName(int collectionId, bool isFirstStage = false, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<string>> GetMessageKeysAsync(int collectionId, bool isFirstStage, CancellationToken cancellationToken = default(CancellationToken));
    }
}
