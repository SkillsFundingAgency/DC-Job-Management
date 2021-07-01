using System.Threading.Tasks;
using ESFA.DC.JobScheduler.Interfaces.Models;

namespace ESFA.DC.JobScheduler.Interfaces
{
    public interface IMessagingService
    {
        Task SendMessageAsync(MessageParameters messageParameters);
    }
}