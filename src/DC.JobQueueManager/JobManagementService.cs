using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Converters;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using MoreLinq.Extensions;

namespace ESFA.DC.JobQueueManager
{
    public class JobManagementService : IJobManagementService
    {
        private readonly IJobQueryService _jobQueryService;
        private readonly IOrganisationService _organisationService;
        private readonly IJobConverter _jobConverter;
        private readonly ILogger _logger;

        public JobManagementService(IJobQueryService jobQueryService, IOrganisationService organisationService, IJobConverter jobConverter, ILogger logger)
        {
            _jobQueryService = jobQueryService;
            _organisationService = organisationService;
            _jobConverter = jobConverter;
            _logger = logger;
        }

        public async Task<IEnumerable<FileUploadJob>> GetJobsForAllPeriods(string collectionName, short? statusCode, CancellationToken cancellationToken)
        {
            var fileUploadJobs = new List<FileUploadJob>();

            var previousJobs = await _jobQueryService.GetJobsForAllPeriodsByCollectionAsync(collectionName, statusCode, cancellationToken);
            if (previousJobs != null)
            {
                var providers = await _organisationService.GetAllValidPimsProviders(
                    previousJobs.DistinctBy(d => d.Job.Ukprn.Value)
                        .Select(s => s.Job.Ukprn.Value), cancellationToken);

                foreach (var previousJob in previousJobs)
                {
                    var destination = new FileUploadJob();
                    var provider = providers.SingleOrDefault(s => s.Ukprn == previousJob.Job.Ukprn);
                    if (provider != null)
                    {
                        await _jobConverter.Convert(previousJob, provider, destination);
                        fileUploadJobs.Add(destination);
                    }
                    else
                    {
                        _logger.LogError(
                            $"Error obtaining Provider Information for GetJobsForPreviousPeriod. No provider found in PIMS for ukprn:{previousJob.Job.Ukprn}");
                    }
                }
            }

            return fileUploadJobs;
        }
    }
}
