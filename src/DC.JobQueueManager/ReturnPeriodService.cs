using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Audit.Models.DTOs.Collections;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Extensions;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class ReturnPeriodService : IReturnPeriodService
    {
        private readonly ILogger _logger;
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IAuditFactory _auditFactory;

        public ReturnPeriodService(Func<IJobQueueDataContext> contextFactory, ILogger logger, IAuditFactory auditFactory)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _auditFactory = auditFactory;
        }

        public async Task<IEnumerable<ReturnPeriod>> GetReturnPeriodsForCollectionAsync(int collectionId)
        {
            using (var context = _contextFactory())
            {
                return await context.ReturnPeriod
                    .Where(rp => rp.Collection.CollectionId == collectionId)
                    .Select(rp => new ReturnPeriod()
                    {
                        ReturnPeriodId = rp.ReturnPeriodId,
                        PeriodNumber = rp.PeriodNumber,
                        CollectionName = rp.Collection.Name,
                        StartDateTimeUtc = rp.StartDateTimeUtc,
                        EndDateTimeUtc = rp.EndDateTimeUtc,
                        CalendarMonth = rp.CalendarMonth,
                        CalendarYear = rp.CalendarYear
                    })
                    .ToListAsync();
            }
        }

        public async Task<ReturnPeriod> GetReturnPeriodAsync(int id)
        {
            using (var context = _contextFactory())
            {
                return await context.ReturnPeriod
                    .Select(s => new ReturnPeriod()
                    {
                        ReturnPeriodId = s.ReturnPeriodId,
                        PeriodNumber = s.PeriodNumber,
                        CollectionName = s.Collection.Name,
                        CalendarMonth = s.CalendarMonth,
                        CalendarYear = s.CalendarYear,
                        StartDateTimeUtc = s.StartDateTimeUtc,
                        EndDateTimeUtc = s.EndDateTimeUtc,
                        CollectionId = s.CollectionId
                    })
                    .SingleOrDefaultAsync(s => s.ReturnPeriodId == id);
            }
        }

        public async Task<DateTime> GetPeriodEndEndDate(string collectionName, int periodNumber)
        {
            using (var context = _contextFactory())
            {
                var collectionId = await context.Collection
                    .Where(c => c.Name == collectionName)
                    .Select(c => c.CollectionId)
                    .SingleAsync();

                var period = await context.ReturnPeriod
                    .Where(rp => rp.CollectionId == collectionId && rp.PeriodNumber == periodNumber)
                    .SingleAsync();

                return period.EndDateTimeUtc;
            }
        }

        public async Task<bool> UpdateReturnPeriod(ReturnPeriod returnPeriod, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await ProvideAuditUpdateRPDataFunc(returnPeriod), context);
                await audit.BeforeAsync(cancellationToken);

                var rp = await context.ReturnPeriod.FindAsync(returnPeriod.ReturnPeriodId);
                if (rp == null)
                {
                    _logger.LogError($"Unable to find return period with id: {returnPeriod.ReturnPeriodId}");
                    return false;
                }

                rp.StartDateTimeUtc = returnPeriod.StartDateTimeUtc;
                rp.EndDateTimeUtc = returnPeriod.EndDateTimeUtc;

                await context.SaveChangesAsync(cancellationToken);

                await audit.AfterAndSaveAsync(cancellationToken);
                return true;
            }
        }

        public async Task<IEnumerable<ReturnPeriod>> GetReturnPeriodsUpToGivenDateForCollectionTypeAsync(DateTime givenDateUtc, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.ReturnPeriod
                    .Where(w => w.Collection.CollectionType.Type == collectionType &&
                                w.StartDateTimeUtc <= givenDateUtc)
                    .OrderByDescending(o => o.Collection.CollectionYear)
                    .ThenByDescending(t => t.PeriodNumber)
                    .Select(s => new ReturnPeriod()
                    {
                        CollectionYear = s.Collection.CollectionYear.GetValueOrDefault(),
                        EndDateTimeUtc = s.EndDateTimeUtc,
                        PeriodNumber = s.PeriodNumber,
                        CollectionId = s.CollectionId,
                        CalendarYear = s.CalendarYear,
                        StartDateTimeUtc = s.StartDateTimeUtc,
                        CalendarMonth = s.CalendarMonth,
                        CollectionName = s.Collection.Name,
                        IsOpen = s.IsCurrentPeriod(givenDateUtc),
                        ReturnPeriodId = s.ReturnPeriodId
                    })
                    .ToListAsync(cancellationToken);
            }
        }

        private async Task<Func<IJobQueueDataContext, Task<ManagingPeriodCollectionDTO>>> ProvideAuditUpdateRPDataFunc(ReturnPeriod returnPeriod)
        {
            return async c => await c.ReturnPeriod
               .Select(s => new ManagingPeriodCollectionDTO()
               {
                   ReturnPeriodId = s.ReturnPeriodId,
                   ClosingDateUTC = s.EndDateTimeUtc,
                   OpeningDateUTC = s.StartDateTimeUtc
               })
               .SingleOrDefaultAsync(s => s.ReturnPeriodId == returnPeriod.ReturnPeriodId);
        }
    }
}
