using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces.DataAccess
{
    public interface IScheduleRepository
    {
        Task<List<JobSchedule>> GetJobSchedules(IEnumerable<string> collectionNames);
    }
}