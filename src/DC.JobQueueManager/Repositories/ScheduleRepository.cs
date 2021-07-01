using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces.DataAccess;
using ESFA.DC.JobQueueManager.Interfaces.ExternalData;
using ESFA.DC.Jobs.Model;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly IScheduleService _scheduleService;
        private readonly Func<IJobQueueDataContext> _jobQueueContextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ScheduleRepository(
            IScheduleService scheduleService,
            Func<IJobQueueDataContext> jobQueueContextFactory,
            IDateTimeProvider dateTimeProvider)
        {
            _scheduleService = scheduleService;
            _jobQueueContextFactory = jobQueueContextFactory;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<List<JobSchedule>> GetJobSchedules(IEnumerable<string> collectionNames)
        {
            List<Schedule> schedules;
            var jobSchedules = new List<JobSchedule>();

            using (var context = _jobQueueContextFactory())
            {
                schedules = await context.Schedule
                    .Include(s => s.Collection)
                    .Where(s => collectionNames.Any(c => c.Equals(s.Collection.Name, StringComparison.OrdinalIgnoreCase)))
                    .ToListAsync();
            }

            foreach (var schedule in schedules)
            {
                var jobSchedule = new JobSchedule
                {
                    Status = schedule.Paused ? "Paused" : schedule.Enabled ? "Enabled" : "Disabled",
                    DisplayName = schedule.Collection == null ? string.Empty : schedule.Collection.Name,
                    NextRunDue = _scheduleService.GetNextRun(_dateTimeProvider.GetNowUtc(), schedule, false)
                };

                // scheduler works weird and will return in a daily fashion ... if it returns today's that has already run then get tomorrow's schedule
                if (jobSchedule.NextRunDue < _dateTimeProvider.GetNowUtc())
                {
                    jobSchedule.NextRunDue = _scheduleService.GetNextRun(_dateTimeProvider.GetNowUtc().AddDays(1), schedule, false);
                }

                if (jobSchedules.All(js => js.DisplayName != jobSchedule.DisplayName))
                {
                    jobSchedules.Add(jobSchedule);
                    continue;
                }

                // can have multiple schedules per collection - need to find next to run
                var collectionSchedule = jobSchedules.Single(js => js.DisplayName == jobSchedule.DisplayName);
                if (collectionSchedule.NextRunDue > jobSchedule.NextRunDue)
                {
                    collectionSchedule.NextRunDue = jobSchedule.NextRunDue;
                }
            }

            return jobSchedules;
        }
    }
}