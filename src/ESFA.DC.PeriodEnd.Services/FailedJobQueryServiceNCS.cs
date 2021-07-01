using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;

namespace ESFA.DC.PeriodEnd.Services
{
    public class FailedJobQueryServiceNCS : IFailedJobQueryServiceNCS
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly Func<IOrganisationsContext> _orgContextFactory;
        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FailedJobQueryServiceNCS(
            Func<IJobQueueDataContext> contextFactory,
            Func<IOrganisationsContext> orgContextFactory,
            ILogger logger,
            IDateTimeProvider dateTimeProvider)
        {
            _contextFactory = contextFactory;
            _orgContextFactory = orgContextFactory;
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<List<FailedJob>> GetFailedJobsPerPeriod(int collectionYear, int periodNumber)
        {
            var failedJobs = new List<FailedJob>();

            using (var context = _contextFactory())
            using (var orgContext = _orgContextFactory())
            {
                var result = (await context.FromSqlAsync<FailedJob>(
                    CommandType.StoredProcedure,
                    "GetNCSLatestFailedJobsPerCollectionPerPeriod",
                    new { collectionYear, periodNumber })).ToList();

                foreach (var failedJob in result)
                {
                    failedJob.OrganisationName =
                        orgContext.OrgDetails.FirstOrDefault(o => o.Ukprn == failedJob.Ukprn)?.Name;
                    failedJob.DateTimeSubmitted = _dateTimeProvider.ConvertUtcToUk(failedJob.DateTimeSubmitted ?? DateTime.MinValue);
                    failedJobs.Add(failedJob);
                }
            }

            return failedJobs;
        }
    }
}
