using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.Jobs.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces.DataAccess;
using ESFA.DC.JobQueueManager.Interfaces.ExternalData;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobManagement.DataAccess.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly IScheduleService _scheduleService;
        private readonly Func<IJobQueueDataContext> _jobQueueContextFactory;

        public ScheduleRepository(
            IScheduleService scheduleService,
            Func<IJobQueueDataContext> jobQueueContextFactory)
        {
            _scheduleService = scheduleService;
            _jobQueueContextFactory = jobQueueContextFactory;
        }

        public async Task<IEnumerable<JobSchedule>> GetJobSchedules(IEnumerable<string> collectionNames)
        {
            using (var context = _jobQueueContextFactory())
            {
                return await context.Schedule
                    .Include(s => s.Collection)
                    .Where(s => collectionNames
                        .Any(c => c.Equals(s.Collection.Name, StringComparison.OrdinalIgnoreCase)))
                    .Select(s => new JobSchedule
                    {
                        Status = s.Paused ? "Paused" : s.Enabled ? "Enabled" : "Disabled",
                        DisplayName = s.Collection.Description,
                        NextRunDue = _scheduleService.GetNextRun(DateTime.UtcNow, s, false)
                    })
                    .ToListAsync();
            }
        }
    }
}