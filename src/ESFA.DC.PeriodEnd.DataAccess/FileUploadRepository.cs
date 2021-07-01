using System;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;

namespace ESFA.DC.PeriodEnd.DataAccess
{
    public class FileUploadRepository : IFileUploadRepository
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;

        public FileUploadRepository(
            Func<IJobQueueDataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
    }
}