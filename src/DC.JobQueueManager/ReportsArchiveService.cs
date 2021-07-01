using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class ReportsArchiveService : IReportsArchiveService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;

        public ReportsArchiveService(Func<IJobQueueDataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ReportsArchive>> GetAllReportsArchivesAsync(CancellationToken cancellationToken, long ukprn)
        {
            var result = new List<ReportsArchive>();

            using (IJobQueueDataContext context = _contextFactory())
            {
                var query = context.ReportsArchive.Where(x => x.Ukprn == ukprn && x.InSld);

                result = await query.Select(x => new ReportsArchive
                    {
                        CollectionType = x.CollectionType.Type,
                        CollectionTypeId = x.CollectionTypeId,
                        InSld = x.InSld,
                        Period = x.Period,
                        UploadedBy = x.UploadedBy,
                        UploadedDateTimeUtc = x.UploadedDateTimeUtc,
                        Year = x.Year,
                        FileName = x.FileName
                    }).OrderByDescending(x => x.Year).ThenByDescending(x => x.Period).ToListAsync(cancellationToken);
            }

            return result;
        }

        public async Task<IEnumerable<long>> GetProvidersWithDataAsync(CancellationToken cancellationToken, IEnumerable<long> ukprns)
        {
            IEnumerable<long> result;

            using (IJobQueueDataContext context = _contextFactory())
            {
                result = await context.ReportsArchive.Where(x => ukprns.Contains(x.Ukprn))
                    .Select(x => x.Ukprn)
                    .Distinct()
                    .ToListAsync(cancellationToken);
            }

            return result;
        }
    }
}
