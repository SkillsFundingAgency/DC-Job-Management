using System.Threading.Tasks;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IAppsReturnPeriodHelper
    {
        Task<int> GetReturnPeriod(int calendarYear, int periodNumber);
    }
}