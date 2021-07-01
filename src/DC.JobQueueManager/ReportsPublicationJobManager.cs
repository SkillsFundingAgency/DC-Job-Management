using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Converters;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;
using Job = ESFA.DC.JobQueueManager.Data.Entities.Job;
using JobStatusType = ESFA.DC.Jobs.Model.Enums.JobStatusType;

namespace ESFA.DC.JobQueueManager
{
    public sealed class ReportsPublicationJobManager : AbstractJobManager, IUpdateJobManager<ReportsPublicationJob>
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;
        private readonly ICollectionService _collectionService;
        private readonly IJobConverter _jobConverter;

        public ReportsPublicationJobManager(
            Func<IJobQueueDataContext> contextFactory,
            IDateTimeProvider dateTimeProvider,
            ILogger logger,
            ICollectionService collectionService,
            IJobConverter jobConverter)
        : base(contextFactory)
        {
            _contextFactory = contextFactory;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _collectionService = collectionService;
            _jobConverter = jobConverter;
        }

        public async Task<long> AddJob(ReportsPublicationJob publicationJob)
        {
            if (publicationJob == null)
            {
                throw new ArgumentNullException();
            }

            var collection = await _collectionService.GetCollectionFromNameAsync(CancellationToken.None, publicationJob.CollectionName);

            if (collection == null)
            {
                throw new ArgumentException($"collection name is not valid : collection name : {publicationJob.CollectionName}");
            }

            using (var context = _contextFactory())
            {
                var entity = new Job
                {
                    DateTimeCreatedUtc = _dateTimeProvider.GetNowUtc(),
                    CollectionId = collection.CollectionId,
                    Priority = publicationJob.Priority,
                    Status = (short)publicationJob.Status,
                    CreatedBy = publicationJob.CreatedBy,
                    NotifyEmail = publicationJob.NotifyEmail,
                    CrossLoadingStatus = (await IsCrossLoadingEnabled(collection.CollectionId)) ? (short)JobStatusType.Ready : (short?)null,
                    Ukprn = publicationJob.Ukprn,
                };

                var metaEntity = new Data.Entities.ReportsPublicationJobMetaData()
                {
                    SourceContainerName = $"{collection.CollectionTitle.Substring(0, 3).ToLower()}{collection.CollectionYear}",
                    SourceFolderKey = publicationJob.SourceFolderKey,
                    Job = entity,
                    PeriodNumber = publicationJob.PeriodNumber,
                    StorageReference = collection.StorageReference,
                };

                context.Job.Add(entity);
                context.ReportsPublicationJobMetaData.Add(metaEntity);

                await context.SaveChangesAsync();

                return entity.JobId;
            }
        }

        public async Task<IEnumerable<ReportsPublicationJob>> GetAllJobs()
        {
            using (var context = _contextFactory())
            {
                return await context.ReportsPublicationJobMetaData
                    .Include(x => x.Job)
                    .ThenInclude(x => x.Collection)
                    .Select(md => new ReportsPublicationJob
                    {
                        StorageReference = md.StorageReference,
                        PeriodNumber = md.PeriodNumber,
                        SourceContainerName = md.SourceContainerName,
                        SourceFolderKey = md.SourceFolderKey,
                        JobId = md.JobId,
                        CollectionName = md.Job.Collection.Name,
                        Status = (JobStatusType)md.Job.Status,
                        Priority = md.Job.Priority,
                        DateTimeCreatedUtc = md.Job.DateTimeCreatedUtc,
                        DateTimeUpdatedUtc = md.Job.DateTimeUpdatedUtc,
                        RowVersion = md.Job.RowVersion == null ? null : System.Convert.ToBase64String(md.Job.RowVersion),
                        CreatedBy = md.Job.CreatedBy,
                        NotifyEmail = md.Job.NotifyEmail,
                        CrossLoadingStatus = null,
                        Ukprn = md.Job.Ukprn.GetValueOrDefault()
                    })
                    .ToListAsync();
            }
        }

        public Task<bool> UpdateJob(ReportsPublicationJob publicationJob)
        {
            throw new NotImplementedException();
        }
    }
}