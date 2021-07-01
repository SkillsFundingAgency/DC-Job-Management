using ESFA.DC.JobQueueManager.Data.ReadOnlyEntities;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager.Data
{
    public partial class JobQueueDataContext : IJobQueueDataContext
    {
        public DbQuery<ReadOnlyJob> ReadOnlyJob { get; set; }

        public DbQuery<ReadOnlyJobProcessing> ReadOnlyJobProcessing { get; set; }

        public DbQuery<ReadOnlyJobQueued> ReadOnlyJobQueued { get; set; }

        public DbQuery<ReadOnlyJobSubmitted> ReadOnlyJobSubmitted { get; set; }

        public DbQuery<ReadOnlyJobFailedToday> ReadOnlyJobFailedToday { get; set; }

        public DbQuery<ReadOnlyJobSlowFile> ReadOnlyJobSlowFile { get; set; }

        public DbQuery<ReadOnlyJobConcern> ReadOnlyJobConcern { get; set; }
    }
}
