using System;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data.Entities;

namespace ESFA.DC.JobQueueManager.Interfaces.ExternalData
{
    public interface IScheduleService
    {
        Task<bool> CanExecuteSchedule(Schedule schedule, DateTime nowUtc, bool removeOldDates);

        DateTime? GetNextRun(DateTime nowUtc, Schedule schedule, bool removeOldDates);
    }
}
