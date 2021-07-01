using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.JobQueueManager
{
    public class FisReferenceDataJobMetaDataService : IFisReferenceDataJobMetaDataService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly ILogger _logger;

        public FisReferenceDataJobMetaDataService(Func<IJobQueueDataContext> contextFactory, ILogger logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<FisJobMetaData> GetFisJobMetaDataForJobId(long jobId, CancellationToken cancellationToken)
        {
            using (IJobQueueDataContext context = _contextFactory())
            {
                _logger.LogInfo($"Retrieving FisJobMetaData for Job Id {jobId}");
                return context.FisJobMetaData.FirstOrDefault(x => x.JobId == jobId);
            }
        }

        public async Task<int> GetVersionNumberForJobId(long jobId, CancellationToken cancellationToken)
        {
            using (IJobQueueDataContext context = _contextFactory())
            {
                _logger.LogInfo($"Retrieving FisJobMetaData version number for Job Id {jobId}");
                return context.FisJobMetaData.FirstOrDefault(x => x.JobId == jobId).VersionNumber;
            }
        }

        public async Task SetGeneratedDateForJobId(long jobId, DateTime dateTime, CancellationToken cancellationToken)
        {
            using (IJobQueueDataContext context = _contextFactory())
            {
                _logger.LogInfo($"Updating FisJobMetaData GeneratedDate for Job Id {jobId}");
                var job = context.FisJobMetaData.FirstOrDefault(x => x.JobId == jobId);

                job.GeneratedDate = dateTime;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task SetPublishedDateForJobId(long jobId, DateTime dateTime, CancellationToken cancellationToken)
        {
            using (IJobQueueDataContext context = _contextFactory())
            {
                _logger.LogInfo($"Updating FisJobMetaData PublishedDate for Job Id {jobId}");
                var job = context.FisJobMetaData.FirstOrDefault(x => x.JobId == jobId);

                job.PublishedDate = dateTime;

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task SetRemovedFlagForJobId(long jobId, CancellationToken cancellationToken)
        {
            using (IJobQueueDataContext context = _contextFactory())
            {
                _logger.LogInfo($"Updating FisJobMetaData IsRemoved for Job Id {jobId}");
                var job = context.FisJobMetaData.FirstOrDefault(x => x.JobId == jobId);

                job.IsRemoved = true;

                await context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
