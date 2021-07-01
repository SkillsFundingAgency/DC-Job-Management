using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Audit.Models.DTOs.FRM;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Jobs.Model;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class ReportsPublicationJobMetaDataService : IReportsPublicationJobMetaDataService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IAuditFactory _auditFactory;

        public ReportsPublicationJobMetaDataService(Func<IJobQueueDataContext> contextFactory, IAuditFactory auditFactory)
        {
            _auditFactory = auditFactory;
            _contextFactory = contextFactory;
        }

        public async Task<ReportsPublicationJobMetaData> GetFrmReportsJobParameters(long jobId)
        {
            ReportsPublicationJobMetaData result = null;
            using (IJobQueueDataContext context = _contextFactory())
            {
                var data = await context.ReportsPublicationJobMetaData.SingleOrDefaultAsync(x => x.JobId == jobId);
                if (data != null)
                {
                    result = new ReportsPublicationJobMetaData
                    {
                        JobId = jobId,
                        SourceContainerName = data.SourceContainerName,
                        SourceFolderKey = data.SourceFolderKey,
                        PeriodNumber = data.PeriodNumber,
                        StorageReference = data.StorageReference,
                    };
                }
            }

            return result;
        }

        public async Task<IEnumerable<PublishedReportDto>> GetReportsPublicationDataAsync(CancellationToken cancellationToken, string collectionName = null)
        {
            using (var context = _contextFactory())
            {
                var entities = context.ReportsPublicationJobMetaData.Where(x => x.ReportsPublished.GetValueOrDefault());
                if (!string.IsNullOrEmpty(collectionName))
                {
                    entities = entities.Where(x => x.Job.Collection.Name == collectionName);
                }

                var data = await entities.GroupBy(x =>
                        new
                        {
                            x.Job.Collection.Name,
                            x.Job.CollectionId,
                            CollectionYear = x.Job.Collection.CollectionYear.GetValueOrDefault(),
                            x.PeriodNumber
                        })
                    .Select(x =>
                        new PublishedReportDto
                        {
                            CollectionYear = x.Key.CollectionYear,
                            PeriodNumber = x.Key.PeriodNumber,
                            CollectionId = x.Key.CollectionId,
                            CollectionName = x.Key.Name
                        }).ToListAsync(cancellationToken);

                return data;
            }
        }

        public async Task MarkAsPublishedAsync(long jobId, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await ProvideAuditFrmPublishUpdateFlagFunc(jobId), context);
                await audit.BeforeAsync(cancellationToken);
                var entity = await
                    context.ReportsPublicationJobMetaData.FirstOrDefaultAsync(x => x.JobId == jobId, cancellationToken);

                if (entity == null)
                {
                    throw new ArgumentException($"job id: {jobId} is not valid for this operation");
                }

                entity.ReportsPublished = true;
                await context.SaveChangesAsync(cancellationToken);
                await audit.AfterAndSaveAsync(cancellationToken);
            }
        }

        public async Task MarkAsUnPublishedAsync(string collectionName, int period, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await ProvideAuditFRMUnpublishDataFunc(period, collectionName), context);

                var entity = await
                    context.ReportsPublicationJobMetaData.FirstOrDefaultAsync(
                        x => x.Job.Collection.Name == collectionName
                             && x.PeriodNumber == period && x.ReportsPublished.GetValueOrDefault(), cancellationToken);

                if (entity != null)
                {
                    entity.ReportsPublished = false;
                    await context.SaveChangesAsync(cancellationToken);
                }

                await audit.AfterAndSaveAsync(cancellationToken);
            }
        }

        private async Task<Func<IJobQueueDataContext, Task<FrmUnpublishDTO>>> ProvideAuditFRMUnpublishDataFunc(int period, string collectionName)
        {
            return
                async c => await c.ReportsPublicationJobMetaData
                    .Select(s => new FrmUnpublishDTO()
                    {
                        Folder = s.StorageReference,
                        Period = s.PeriodNumber,
                        CollectionName = collectionName
                    }).FirstOrDefaultAsync(s => s.CollectionName == collectionName && s.Period == period);
        }

        private async Task<Func<IJobQueueDataContext, Task<FRMPublishUpdateFlagDTO>>> ProvideAuditFrmPublishUpdateFlagFunc(long jobid)
        {
            return
                async c => await c.ReportsPublicationJobMetaData
                    .Select(s => new FRMPublishUpdateFlagDTO()
                    {
                        JobId = s.JobId,
                        IsPublished = s.ReportsPublished
                    }).FirstOrDefaultAsync(s => s.JobId == jobid);
        }
    }
}
