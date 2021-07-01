using System;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public abstract class AbstractJobManager
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;

        protected AbstractJobManager(Func<IJobQueueDataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> IsCrossLoadingEnabled(int collectionId)
        {
            using (var context = _contextFactory())
            {
                var entity = await context.Collection.SingleOrDefaultAsync(x => x.CollectionId == collectionId);
                return entity != null && entity.CrossloadingEnabled.GetValueOrDefault();
            }
        }
    }
}
