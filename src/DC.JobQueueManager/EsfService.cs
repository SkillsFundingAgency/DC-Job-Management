using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class EsfService : IEsfService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly ILogger _logger;

        public EsfService(
            Func<IJobQueueDataContext> contextFactory,
            ILogger logger,
            IAuditFactory auditFactory)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task UpdateJobMetaDataAsync(EsfJobMetaData esfJobMetaData, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Call to EsfService.UpdateJobMetaDataAsync for JobId:{esfJobMetaData.JobId}");

            using (var context = _contextFactory())
            {
                var esfMetaDataRecord = await context.EsfJobMetaData.SingleOrDefaultAsync(s => s.JobId == esfJobMetaData.JobId, cancellationToken);

                if (esfMetaDataRecord != null)
                {
                    esfMetaDataRecord.PublishedDate = esfJobMetaData.PublishedDate;
                    await context.SaveChangesAsync(cancellationToken);
                    _logger.LogInfo($"EsfJobMetaData record with JobId:{esfJobMetaData.JobId} updated");
                }
                else
                {
                    _logger.LogError($"EsfJobMetaData record with JobId:{esfJobMetaData.JobId} not found");
                }
            }
        }

        public async Task CreateJobMetaDataAsync(EsfJobMetaData esfJobMetaData, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Call to EsfService.CreateJobMetaDataAsync for JobId:{esfJobMetaData.JobId}");

            using (var context = _contextFactory())
            {
                if (await context.EsfJobMetaData.AnyAsync(a => a.JobId == esfJobMetaData.JobId, cancellationToken))
                {
                    throw new ArgumentException($"Call to EsfService.CreateJobMetaDataAsync attempt to add duplicate JobId:{esfJobMetaData.JobId}");
                }

                var esfMetaDataRecord = new Data.Entities.EsfJobMetaData()
                {
                    JobId = esfJobMetaData.JobId,
                    ContractReferenceNumber = esfJobMetaData.ContractReferenceNumber,
                    PublishedDate = null
                };

                await context.EsfJobMetaData.AddAsync(esfMetaDataRecord, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);
            }

            _logger.LogInfo($"Call to EsfService.CreateJobMetaDataAsync Complete for JobId:{esfJobMetaData.JobId}");
        }
    }
}
