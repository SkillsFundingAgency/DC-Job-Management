using System;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model.Enums;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class CollectionEmailTemplateManager : ICollectionEmailTemplateManager
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;

        public CollectionEmailTemplateManager(Func<IJobQueueDataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<string> GetTemplate(int collectionId, JobStatusType? jobStatus = null)
        {
            using (IJobQueueDataContext context = _contextFactory())
            {
                var emailTemplate = await
                    context.JobEmailTemplate.SingleOrDefaultAsync(x => x.CollectionId == collectionId
                                                                       && x.Active.Value
                                                                       && (jobStatus == null || x.JobStatus == (short)jobStatus.Value));

                return emailTemplate?.TemplateOpenPeriod ?? string.Empty;
            }
        }
    }
}
