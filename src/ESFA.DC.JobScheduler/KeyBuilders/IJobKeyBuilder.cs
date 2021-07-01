using System.Threading.Tasks;
using ESFA.DC.JobContext.Interface;

namespace ESFA.DC.JobScheduler.KeyBuilders
{
    public interface IJobKeyBuilder<T, U>
        where T : IJobContextMessage
    {
        Task AddKeys(T message, U data);
    }
}
