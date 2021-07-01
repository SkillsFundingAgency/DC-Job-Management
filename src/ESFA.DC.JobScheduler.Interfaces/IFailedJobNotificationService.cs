using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobScheduler.Interfaces
{
    public interface IFailedJobNotificationService
    {
        Task SendMessageAsync(SubmittedJob job);
    }
}
