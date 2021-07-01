using System.Threading.Tasks;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IESFReturnPeriodHelper
    {
        Task<int> GetESFReturnPeriod(int calendarYear, int periodNumber);
    }
}