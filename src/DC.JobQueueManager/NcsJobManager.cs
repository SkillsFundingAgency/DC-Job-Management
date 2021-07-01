using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Converters;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using Job = ESFA.DC.JobQueueManager.Data.Entities.Job;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.JobQueueManager
{
    public sealed class NcsJobManager : AbstractJobManager, INcsJobManager
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICollectionService _collectionService;
        private readonly IJobConverter _jobConverter;

        public NcsJobManager(
            Func<IJobQueueDataContext> contextFactory,
            IDateTimeProvider dateTimeProvider,
            IEmailNotifier emailNotifier,
            ILogger logger,
            ICollectionService collectionService,
            IJobConverter jobConverter)
            : base(contextFactory)
        {
            _contextFactory = contextFactory;
            _dateTimeProvider = dateTimeProvider;
            _collectionService = collectionService;
            _jobConverter = jobConverter;
        }

        public async Task<long> AddJob(NcsJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException();
            }

            var collection = await _collectionService.GetCollectionFromNameAsync(CancellationToken.None, job.CollectionName);

            if (collection == null)
            {
                throw new ArgumentException($"collection name is not valid : collection name : {job.CollectionName}");
            }

            using (var context = _contextFactory())
            {
                var entity = new Job
                {
                    Ukprn = job.Ukprn,
                    DateTimeCreatedUtc = _dateTimeProvider.GetNowUtc(),
                    CollectionId = collection.CollectionId,
                    Priority = job.Priority,
                    Status = (short)job.Status,
                    CreatedBy = job.CreatedBy,
                    NotifyEmail = job.NotifyEmail,
                    CrossLoadingStatus = (await IsCrossLoadingEnabled(collection.CollectionId)) ? (short)JobStatusType.Ready : (short?)null,
                };

                var metaEntity = new NcsJobMetaData()
                {
                    ExternalJobId = job.ExternalJobId,
                    TouchpointId = job.TouchpointId,
                    ExternalTimestamp = job.ExternalTimestamp,
                    DssContainer = job.DssContainer,
                    ReportFileName = job.ReportFileName,
                    ReportEndDate = job.ReportEndDate,
                    PeriodNumber = job.Period,
                    Job = entity,
                };

                context.Job.Add(entity);
                context.NcsJobMetaData.Add(metaEntity);

                await context.SaveChangesAsync();

                return entity.JobId;
            }
        }

        public async Task<IEnumerable<NcsJob>> GetAllJobs()
        {
            using (var context = _contextFactory())
            {
                var entities = await context.NcsJobMetaData.Include(x => x.Job).ToListAsync();
                return await ConvertJobs(entities);
            }
        }

        public async Task<NcsJob> GetJobById(long jobId)
        {
            if (jobId == 0)
            {
                throw new ArgumentException("Job id can not be 0");
            }

            using (var context = _contextFactory())
            {
                var entity = await context.NcsJobMetaData.Include(x => x.Job).SingleOrDefaultAsync(x => x.JobId == jobId);
                if (entity == null)
                {
                    throw new ArgumentException($"Job id {jobId} does not exist");
                }

                var job = new NcsJob();
                await _jobConverter.Convert(entity, job);
                return job;
            }
        }

        public async Task SendEmailNotification(long jobId)
        {
            // Emails not needed for NCS??? TBC
        }

        private async Task<IEnumerable<NcsJob>> ConvertJobs(IEnumerable<NcsJobMetaData> entities)
        {
            var items = new List<NcsJob>();
            foreach (var entity in entities)
            {
                var model = new NcsJob();
                await _jobConverter.Convert(entity, model);
                items.Add(model);
            }

            return items;
        }
    }
}
