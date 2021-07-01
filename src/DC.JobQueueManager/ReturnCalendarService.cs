using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class ReturnCalendarService : IReturnCalendarService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ReturnCalendarService(Func<IJobQueueDataContext> contextFactory, IDateTimeProvider dateTimeProvider)
        {
            _contextFactory = contextFactory;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ReturnPeriod> GetPeriodAsync(int collectionId, DateTime dateTimeUtc)
        {
            using (var context = _contextFactory())
            {
                var data = await context.ReturnPeriod
                    .Include(x => x.Collection)
                    .Where(x =>
                        x.CollectionId == collectionId &&
                        dateTimeUtc >= x.StartDateTimeUtc
                        && dateTimeUtc <= x.EndDateTimeUtc)
                    .FirstOrDefaultAsync();

                if (data != null)
                {
                    return Convert(data);
                }
            }

            return null;
        }

        public async Task<ReturnPeriod> GetPeriodAsync(int collectionId, int periodNumber)
        {
            using (var context = _contextFactory())
            {
                var data = await context.ReturnPeriod
                    .Include(x => x.Collection)
                    .Where(x =>
                        x.CollectionId == collectionId && x.PeriodNumber == periodNumber)
                    .FirstOrDefaultAsync();

                if (data != null)
                {
                    return Convert(data);
                }
            }

            return null;
        }

        public async Task<ReturnPeriod> GetPeriodAsync(string collectionName, DateTime dateTimeUtc)
        {
            using (var context = _contextFactory())
            {
                var data = await context.ReturnPeriod
                    .Include(x => x.Collection)
                    .Where(x =>
                        x.Collection.Name == collectionName &&
                        dateTimeUtc >= x.StartDateTimeUtc
                        && dateTimeUtc <= x.EndDateTimeUtc)
                    .FirstOrDefaultAsync();

                if (data != null)
                {
                    return Convert(data);
                }
            }

            return null;
        }

        public async Task<ReturnPeriod> GetPreviousPeriodAsync(string collectionName, CancellationToken cancellationToken)
        {
            var dateTimeUtc = _dateTimeProvider.GetNowUtc();
            var currentPeriod = await GetPeriodAsync(collectionName, dateTimeUtc);

            if (currentPeriod == null)
            {
                // outside of a period, return the previous closest period
                return await GetPreviousPeriodAsync(collectionName, dateTimeUtc);
            }

            if (currentPeriod.PeriodNumber > 1)
            {
                // In a period, return the previous one.
                using (var context = _contextFactory())
                {
                    var data = await context.ReturnPeriod
                        .Include(i => i.Collection)
                        .SingleOrDefaultAsync(
                            w => w.Collection.Name == collectionName && w.PeriodNumber == currentPeriod.PeriodNumber - 1,
                            cancellationToken);

                    return data == null ? null : Convert(data);
                }
            }

            return null;
        }

        public async Task<ReturnPeriod> GetPreviousPeriodAsync(string collectionName, DateTime dateTimeUtc)
        {
            using (var context = _contextFactory())
            {
                var data = await context.ReturnPeriod
                    .Include(x => x.Collection)
                    .Where(x =>
                        x.Collection.Name == collectionName &&
                        x.StartDateTimeUtc < dateTimeUtc)
                    .OrderByDescending(x => x.StartDateTimeUtc)
                    .FirstOrDefaultAsync();

                return Convert(data);
            }
        }

        public async Task<ReturnPeriod> GetCurrentPeriodAsync(string collectionName)
        {
            var currentDateTime = _dateTimeProvider.GetNowUtc();
            return await GetPeriodAsync(collectionName, currentDateTime);
        }

        public async Task<ReturnPeriod> GetNextPeriodAsync(string collectionName)
        {
            var currentDateTime = _dateTimeProvider.GetNowUtc();
            using (var context = _contextFactory())
            {
                var data = await context.ReturnPeriod.Include(x => x.Collection).Where(x =>
                        x.Collection.Name == collectionName &&
                        x.StartDateTimeUtc > currentDateTime).OrderBy(x => x.StartDateTimeUtc)
                    .FirstOrDefaultAsync();

                return Convert(data);
            }
        }

        public async Task<ReturnPeriod> GetNextPeriodAsync(int collectionId)
        {
            var currentDateTime = _dateTimeProvider.GetNowUtc();
            using (var context = _contextFactory())
            {
                var data = await context.ReturnPeriod
                    .Include(x => x.Collection)
                    .Where(x =>
                        x.CollectionId == collectionId &&
                        x.StartDateTimeUtc > currentDateTime).OrderBy(x => x.StartDateTimeUtc)
                    .FirstOrDefaultAsync();

                return Convert(data);
            }
        }

        public async Task<List<ReturnPeriod>> GetOpenPeriodsAsync(DateTime? dateTimeUtc = null, string collectionType = CollectionTypeConstants.Ilr)
        {
            var result = new List<ReturnPeriod>();

            dateTimeUtc = dateTimeUtc ?? _dateTimeProvider.GetNowUtc();

            using (var context = _contextFactory())
            {
                var data = await context.ReturnPeriod
                    .Include(x => x.Collection)
                    .ThenInclude(x => x.CollectionType)
                    .Where(x =>
                        x.Collection.CollectionType.Type == collectionType &&
                        dateTimeUtc >= x.StartDateTimeUtc
                        && dateTimeUtc <= x.EndDateTimeUtc).ToListAsync();

                if (data != null)
                {
                    foreach (var returnPeriod in data)
                    {
                        result.Add(Convert(returnPeriod));
                    }
                }
            }

            return result;
        }

        public async Task<ReturnPeriod> GetRecentlyClosedPeriodAsync(DateTime? dateTimeUtc = null, string collectionType = CollectionTypeConstants.Ilr)
        {
            dateTimeUtc = dateTimeUtc ?? _dateTimeProvider.GetNowUtc();

            using (var context = _contextFactory())
            {
                var data = await context.ReturnPeriod
                    .Include(x => x.Collection)
                    .ThenInclude(x => x.CollectionType)
                    .Where(x =>
                        x.Collection.CollectionType.Type == collectionType &&
                        dateTimeUtc >= x.EndDateTimeUtc)
                    .OrderByDescending(x => x.EndDateTimeUtc)
                    .FirstOrDefaultAsync();

                if (data != null)
                {
                    return Convert(data);
                }
            }

            return null;
        }

        public async Task<YearPeriod> GetPeriodEndPeriod(string collectionType)
        {
            var dateTimeUtc = _dateTimeProvider.GetNowUtc();

            using (var context = _contextFactory())
            {
                var filteredReturnPeriods = await context.ReturnPeriod
                    .Include(x => x.Collection)
                    .Where(x =>
                        x.Collection.CollectionType.Type == collectionType)
                    .OrderByDescending(x => x.EndDateTimeUtc)
                    .ToListAsync();

                bool isClosed = false;
                var closed = filteredReturnPeriods.FirstOrDefault(x => dateTimeUtc >= x.EndDateTimeUtc);

                if (closed != null)
                {
                    var closedPeriodEnds = await context.PeriodEnd.Where(x => x.Closed && x.Period.Collection.CollectionType.Type == collectionType).Select(x => x.PeriodId).ToListAsync();

                    // Period end has not previously run
                    if (!closedPeriodEnds.Any())
                    {
                        // No period ends run so take latest closed.
                        return new YearPeriod
                        {
                            Period = closed.PeriodNumber,
                            Year = closed.Collection.CollectionYear.GetValueOrDefault(),
                            PeriodClosed = true
                        };
                    }

                    // Walk through the available periods until no more closed period ends
                    while (closedPeriodEnds.Any(x => x == closed.ReturnPeriodId))
                    {
                        int index = filteredReturnPeriods.IndexOf(closed) - 1;
                        if (index < 0)
                        {
                            break;
                        }

                        closed = filteredReturnPeriods[index];
                    }

                    // if there is no currently open period
                    if (closed.EndDateTimeUtc < dateTimeUtc)
                    {
                        isClosed = true;
                    }
                }
                else
                {
                    closed = filteredReturnPeriods.FirstOrDefault(x => x.StartDateTimeUtc < dateTimeUtc);
                }

                if (closed != null)
                {
                    return new YearPeriod
                    {
                        Period = closed.PeriodNumber,
                        Year = closed.Collection.CollectionYear.GetValueOrDefault(),
                        PeriodEndDate = closed.EndDateTimeUtc,
                        PeriodClosed = isClosed
                    };
                }

                return null;
            }
        }

        public async Task<ReturnPeriod> GetPeriodForCollectionType(string collectionTypeName, int collectionYear, int periodNumber)
        {
            using (var context = _contextFactory())
            {
                var returnPeriod = await context.ReturnPeriod.Include(x => x.Collection)
                    .Where(x => x.Collection.CollectionType.Type.Equals(collectionTypeName, StringComparison.OrdinalIgnoreCase)
                                && (collectionYear == 0 || x.Collection.CollectionYear == collectionYear)
                                && x.PeriodNumber == periodNumber)
                    .SingleOrDefaultAsync();

                if (returnPeriod != null)
                {
                    return Convert(returnPeriod);
                }
            }

            return null;
        }

        public async Task<ReturnPeriod[]> GetAllPeriodsAsync(string collectionName = null, string collectionType = null)
        {
            List<ReturnPeriod> result;

            using (var context = _contextFactory())
            {
                result = await context.ReturnPeriod
                    .Include(x => x.Collection)
                    .Where(x =>
                        (string.IsNullOrEmpty(collectionName) || x.Collection.Name.Equals(collectionName, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrEmpty(collectionType) || x.Collection.CollectionType.Type.Equals(collectionType, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(x => x.PeriodNumber)
                    .Select(rp => Convert(rp))
                    .ToListAsync();
            }

            return result.ToArray();
        }

        public async Task<bool> IsReferenceDataCollectionExpiredAsync(string collectionName, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var referenceDataCollectionYear = await context.Collection
                    .Where(c => c.Name == collectionName)
                    .Select(c => c.CollectionYear).FirstOrDefaultAsync(cancellationToken);

                if (referenceDataCollectionYear == null)
                {
                    return false;
                }

                var returnPeriod = await context.ReturnPeriod
                    .Where(rp => rp.Collection.CollectionType.Type == CollectionTypeConstants.Ilr && rp.Collection.CollectionYear == referenceDataCollectionYear)
                    .OrderByDescending(rp => rp.PeriodNumber)
                    .FirstOrDefaultAsync(cancellationToken);

                return returnPeriod.EndDateTimeUtc < _dateTimeProvider.GetNowUtc().AddMonths(-2);
            }
        }

        public async Task<ReturnPeriod> GetNextClosingPeriodAsync(string collectionType, CancellationToken cancellationToken)
        {
            var currentDateTime = _dateTimeProvider.GetNowUtc();
            using (var context = _contextFactory())
            {
                var data = await context.ReturnPeriod
                    .Include(x => x.Collection)
                    .Where(x =>
                        x.Collection.CollectionType.Type == collectionType &&
                        x.EndDateTimeUtc > currentDateTime)
                    .OrderBy(x => x.EndDateTimeUtc)
                    .FirstOrDefaultAsync(cancellationToken);

                return Convert(data);
            }
        }

        private ReturnPeriod Convert(Data.Entities.ReturnPeriod data)
        {
            if (data == null)
            {
                return null;
            }

            var period = new ReturnPeriod
            {
                PeriodNumber = data.PeriodNumber,
                EndDateTimeUtc = data.EndDateTimeUtc,
                StartDateTimeUtc = data.StartDateTimeUtc,
                CalendarMonth = data.CalendarMonth,
                CalendarYear = data.CalendarYear,
                CollectionName = data.Collection.Name,
                CollectionYear = data.Collection.CollectionYear.GetValueOrDefault(),
                CollectionId = data.CollectionId,
                ReturnPeriodId = data.ReturnPeriodId
            };

            return period;
        }
    }
}
