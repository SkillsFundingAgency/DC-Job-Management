using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.Audit.Models.DTOs.Collections;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Extensions;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.JobQueueManager.Settings;
using ESFA.DC.Jobs.Model;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Collection = ESFA.DC.CollectionsManagement.Models.Collection;
using CollectionRelatedLink = ESFA.DC.CollectionsManagement.Models.CollectionRelatedLink;

namespace ESFA.DC.JobQueueManager
{
    public class CollectionService : ICollectionService
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IAuditFactory _auditFactory;
        private readonly SemaphoreSlim _updateLock;
        private readonly JobQueueManagerSettings _jobQueueManagerSettings;

        public CollectionService(
            Func<IJobQueueDataContext> contextFactory,
            IDateTimeProvider dateTimeProvider,
            JobQueueManagerSettings jobQueueManagerSettings,
            IAuditFactory auditFactory)
        {
            _dateTimeProvider = dateTimeProvider;
            _auditFactory = auditFactory;
            _jobQueueManagerSettings = jobQueueManagerSettings;
            _contextFactory = contextFactory;
        }

        public async Task<Collection> GetCollectionAsync(CancellationToken cancellationToken, string collectionType)
        {
            using (var context = _contextFactory())
            {
                var data = await context.Collection
                    .Include(i => i.CollectionType)
                    .Where(x => x.CollectionType.Type == collectionType)
                    .OrderByDescending(o => o.CollectionYear)
                    .FirstOrDefaultAsync(cancellationToken);

                if (data == null)
                {
                    return null;
                }

                return Convert(data);
            }
        }

        public async Task<IEnumerable<Collection>> GetAllCollectionTypesAsync(CancellationToken cancellationToken)
        {
            var collections = new List<Collection>();

            using (var context = _contextFactory())
            {
                var data = await context.Collection
                    .Include(i => i.CollectionType)
                    .ToListAsync(cancellationToken);

                data.ForEach(d => collections.Add(Convert(d)));

                return collections;
            }
        }

        public async Task<Collection> GetCollectionAsync(CancellationToken cancellationToken, int id)
        {
            using (var context = _contextFactory())
            {
                return await context.Collection
                    .Where(x => x.CollectionId == id)
                    .Select(c => new Collection()
                    {
                        CollectionId = id,
                        CollectionYear = c.CollectionYear.GetValueOrDefault(),
                        CollectionType = c.CollectionType.Type,
                        CollectionTitle = c.Name,
                        IsOpen = c.IsOpen,
                        StorageReference = c.StorageReference,
                        ProcessingOverride = c.ProcessingOverrideFlag
                    }).SingleOrDefaultAsync(cancellationToken);
            }
        }

        public async Task<Collection> GetCollectionFromNameAsync(CancellationToken cancellationToken, string collectionName)
        {
            using (var context = _contextFactory())
            {
                var data = await context.Collection
                    .Include(x => x.CollectionType)
                    .Where(x => x.Name.Equals(collectionName, StringComparison.InvariantCultureIgnoreCase))
                    .SingleOrDefaultAsync(cancellationToken);

                if (data == null)
                {
                    return null;
                }

                return Convert(data);
            }
        }

        public async Task<Collection> GetCollectionByDateAsync(CancellationToken cancellationToken, string collectionType, DateTime dateTimeUtc)
        {
            using (var context = _contextFactory())
            {
                var collection = await context.ReturnPeriod
                    .Where(w => w.StartDateTimeUtc <= dateTimeUtc && w.EndDateTimeUtc >= dateTimeUtc && w.Collection.CollectionType.Type == collectionType)
                    .Select(s => new Collection()
                    {
                        CollectionId = s.CollectionId,
                        CollectionYear = s.Collection.CollectionYear.GetValueOrDefault(),
                        CollectionType = s.Collection.CollectionType.Type,
                        CollectionTitle = s.Collection.Name,
                        IsOpen = s.Collection.IsOpen,
                        StorageReference = s.Collection.StorageReference,
                        FileNameRegex = s.Collection.FileNameRegex,
                        ProcessingOverride = s.Collection.ProcessingOverrideFlag,
                        Description = s.Collection.Description
                    })
                    .SingleOrDefaultAsync(cancellationToken);

                if (collection == null)
                {
                    collection = await context.ReturnPeriod
                        .Where(w => w.StartDateTimeUtc > dateTimeUtc && w.Collection.CollectionType.Type == collectionType)
                        .OrderBy(o => o.StartDateTimeUtc)
                        .Select(s => new Collection()
                        {
                            CollectionId = s.CollectionId,
                            CollectionYear = s.Collection.CollectionYear.GetValueOrDefault(),
                            CollectionType = s.Collection.CollectionType.Type,
                            CollectionTitle = s.Collection.Name,
                            IsOpen = s.Collection.IsOpen,
                            StorageReference = s.Collection.StorageReference,
                            FileNameRegex = s.Collection.FileNameRegex,
                            ProcessingOverride = s.Collection.ProcessingOverrideFlag,
                            Description = s.Collection.Description
                        })
                        .FirstOrDefaultAsync(cancellationToken);
                }

                return collection;
            }
        }

        public async Task<List<Collection>> GetCollectionsFromNamesAsync(CancellationToken cancellationToken, List<string> collectionNames)
        {
            var collections = new List<Collection>();
            using (var context = _contextFactory())
            {
                var data = await context.Collection
                    .Include(x => x.CollectionType)
                    .Where(x => collectionNames.Contains(x.Name))
                    .ToListAsync(cancellationToken);

                if (data == null)
                {
                    return null;
                }

                data.ForEach(d => collections.Add(Convert(d)));
                return collections;
            }
        }

        /// <summary>
        /// Gets an enumerable of collections by year.
        /// </summary>
        /// <param name="collectionYear">The Collection Year of interest.</param>
        /// <returns>Enumerable of OrganisationCollection.</returns>
        public async Task<IEnumerable<Collection>> GetCollectionsByYearAsync(CancellationToken cancellationToken, int collectionYear)
        {
            using (var context = _contextFactory())
            {
                var now = _dateTimeProvider.GetNowUtc();

                var collections = await context.Collection
                    .Where(w => w.CollectionYear == collectionYear)
                    .Select(s => new Collection()
                    {
                        CollectionId = s.CollectionId,
                        CollectionTitle = s.Name,
                        CollectionType = s.CollectionType.Type,
                        IsManageableInOperations = s.CollectionType.IsManageableInOperations,
                        IsProviderAssignableInOperations = s.CollectionType.IsProviderAssignableInOperations
                    })
                    .ToListAsync(cancellationToken);

                var collectionIds = collections.DistinctBy(d => d.CollectionId).Select(s => s.CollectionId);

                var returnPeriods = await context.ReturnPeriod
                    .Include(i => i.Collection)
                    .Where(rp => collectionIds.Contains(rp.Collection.CollectionId))
                    .ToListAsync(cancellationToken);

                foreach (var collection in collections)
                {
                    var collectionReturnPeriods =
                        returnPeriods.Where(rp => rp.Collection.CollectionId == collection.CollectionId);

                    if (collectionReturnPeriods.Any())
                    {
                        collection.StartDateTimeUtc = collectionReturnPeriods.OrderBy(rp => rp.StartDateTimeUtc).First().StartDateTimeUtc;
                        collection.EndDateTimeUtc = collectionReturnPeriods.OrderBy(rp => rp.EndDateTimeUtc).Last().EndDateTimeUtc;

                        var currentLastOrNextPeriod = GetCurrentLastOrNextPeriod(now, collectionReturnPeriods);
                        collection.IsOpen = IsCollectionOpen(now, currentLastOrNextPeriod);
                        if (currentLastOrNextPeriod.IsCurrentPeriod(now))
                        {
                            collection.OpenPeriodCloseDate = currentLastOrNextPeriod.EndDateTimeUtc;
                            collection.OpenPeriodNumber = currentLastOrNextPeriod.PeriodNumber;
                        }
                        else if (currentLastOrNextPeriod.IsEarlierPeriod(now))
                        {
                            collection.LastPeriodClosedDate = currentLastOrNextPeriod.EndDateTimeUtc;
                            collection.LastPeriodNumber = currentLastOrNextPeriod.PeriodNumber;
                        }
                        else
                        {
                            collection.NextPeriodOpenDateTimeUtc = currentLastOrNextPeriod.StartDateTimeUtc;
                            collection.NextPeriodNumber = currentLastOrNextPeriod.PeriodNumber;
                        }
                    }
                }

                return collections;
            }
        }

        public async Task<IEnumerable<Collection>> GetAllCollectionsByYearAsync(CancellationToken cancellationToken, int collectionYear)
        {
            using (var context = _contextFactory())
            {
                return await context.Collection
                    .Where(w => w.CollectionYear == collectionYear)
                    .Select(s => new Collection()
                    {
                        CollectionId = s.CollectionId,
                        CollectionTitle = s.Name
                    })
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<int>> GetAcademicYearsAsync(CancellationToken cancellationToken, DateTime? dateTimeUtc = null, bool? includeClosed = false)
        {
            dateTimeUtc = dateTimeUtc ?? _dateTimeProvider.GetNowUtc();

            using (var context = _contextFactory())
            {
                var collections = await context.ReturnPeriod
                    .Where(x => x.StartDateTimeUtc <= dateTimeUtc && x.Collection.CollectionType.Type == CollectionTypeConstants.Ilr)
                    .GroupBy(x => new { x.CollectionId })
                    .Select(x => x.Key.CollectionId)
                    .ToListAsync(cancellationToken);

                var result = await context.ReturnPeriod
                        .Where(x => collections.Contains(x.CollectionId))
                        .GroupBy(x => new { x.Collection.CollectionYear })
                        .Select(x => new
                        {
                            CollectionYear = x.Key.CollectionYear.Value,
                            MaxPeriodClosedDate = x.Max(y => y.EndDateTimeUtc)
                        })
                        .Where(y => includeClosed.GetValueOrDefault() || y.MaxPeriodClosedDate >= dateTimeUtc)
                        .Select(x => x.CollectionYear).OrderByDescending(x => x)
                        .ToListAsync();

                return result;
            }
        }

        public async Task<IEnumerable<CollectionRelatedLink>> GetRelatedLinksAsync(CancellationToken cancellationToken, string collectionName)
        {
            using (var context = _contextFactory())
            {
                var result = await context.CollectionRelatedLink.Where(x => x.Collection.Name == collectionName)
                    .Select(x => new CollectionRelatedLink()
                    {
                        Title = x.Title,
                        Url = x.Url,
                        SortOrder = x.SortOrder
                    })
                    .OrderBy(x => x.SortOrder)
                    .ToListAsync(cancellationToken);

                return result;
            }
        }

        public Collection Convert(Data.Entities.Collection entity)
        {
            return new Collection()
            {
                CollectionId = entity.CollectionId,
                CollectionYear = entity.CollectionYear.GetValueOrDefault(),
                CollectionType = entity.CollectionType.Type,
                IsManageableInOperations = entity.CollectionType.IsManageableInOperations,
                IsProviderAssignableInOperations = entity.CollectionType.IsProviderAssignableInOperations,
                CollectionTitle = entity.Name,
                IsOpen = entity.IsOpen,
                StorageReference = entity.StorageReference,
                FileNameRegex = entity.FileNameRegex,
                ProcessingOverride = entity.ProcessingOverrideFlag,
                Description = entity.Description,
                EmailOnJobCreation = entity.EmailOnJobCreation
            };
        }

        public async Task<IEnumerable<int>> GetAvailableCollectionYearsAsync(CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.Collection
                    .Where(w => w.CollectionYear.HasValue)
                    .Select(s => s.CollectionYear.Value)
                    .Distinct()
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<int>> GetCollectionYearsByTypeAsync(CancellationToken cancellationToken, string collectionType)
        {
            using (var context = _contextFactory())
            {
                return await context.Collection
                    .Where(w => w.CollectionType.Type == collectionType && w.CollectionYear.HasValue)
                    .Select(s => s.CollectionYear.Value)
                    .Distinct()
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Collection>> GetCollectionsByTypeAsync(string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.Collection
                    .Where(w => w.CollectionType.Type == collectionType && w.CollectionYear.HasValue)
                    .Select(s => new Collection()
                    {
                        CollectionId = s.CollectionId,
                        CollectionYear = s.CollectionYear.GetValueOrDefault(),
                        CollectionType = s.CollectionType.Type,
                        IsManageableInOperations = s.CollectionType.IsManageableInOperations,
                        IsProviderAssignableInOperations = s.CollectionType.IsProviderAssignableInOperations,
                        CollectionTitle = s.Name,
                        Description = s.Description
                    })
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<DateTime?> GetCollectionStartDateAsync(string collectionName, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.ReturnPeriod
                    .Include(i => i.Collection)
                    .Where(rp => rp.Collection.Name == collectionName
                                 && rp.PeriodNumber == 1)
                    .Select(x => x.StartDateTimeUtc)
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        public async Task<bool> UpdateCollectionProcessingOverrideStatusAsync(CancellationToken cancellationToken, int collectionId, bool? processingOverrideFlag)
        {
            if (collectionId == 0)
            {
                throw new ArgumentException("collectionId can not be 0");
            }

            using (IJobQueueDataContext context = _contextFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await ProvideSetOverrideDTOFunc(collectionId), context);
                await audit.BeforeAsync(cancellationToken);
                var entity = await context.Collection.SingleOrDefaultAsync(x => x.CollectionId == collectionId);
                if (entity == null)
                {
                    throw new ArgumentException($"Collection id {collectionId} does not exist");
                }

                entity.ProcessingOverrideFlag = processingOverrideFlag;
                context.Entry(entity).State = EntityState.Modified;
                await context.SaveChangesAsync(cancellationToken);
                await audit.AfterAndSaveAsync(cancellationToken);
            }

            return true;
        }

        public async Task<bool> IsCollectionOpenByIdWithVarianceAsync(int id, DateTime now, int negativeVarianceInMonths, int positiveVarianceInMonths, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var firstReturnPeriod = await context.ReturnPeriod
                    .OrderBy(o => o.PeriodNumber)
                    .FirstOrDefaultAsync(s => s.CollectionId == id, cancellationToken);

                if (firstReturnPeriod == null)
                {
                    return false;
                }

                var startDate = firstReturnPeriod.StartDateTimeUtc.AddMonths(negativeVarianceInMonths);

                var endDate = (await context.ReturnPeriod
                    .OrderByDescending(o => o.PeriodNumber)
                    .FirstOrDefaultAsync(s => s.CollectionId == id, cancellationToken))
                    .EndDateTimeUtc
                    .AddMonths(positiveVarianceInMonths);

                return (startDate <= now) && (now <= endDate);
            }
        }

        public async Task<IEnumerable<Collection>> GetOpenCollectionsByDateRangeAsync(DateTime startDateTimeUtc, DateTime endDateTimeUtc, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.Collection
                                .Include(i => i.CollectionType)
                                .Where(c => c.ReturnPeriod.Any(rp => rp.StartDateTimeUtc <= startDateTimeUtc) && c.ReturnPeriod.Any(rp => rp.EndDateTimeUtc >= endDateTimeUtc))
                                .Select(c => Convert(c)).ToListAsync();
            }
        }

        public async Task<int> GetCountOfProvidersForCollectionAsync(int collectionId, CancellationToken cancellationToken)
        {
            var parameters = new DynamicParameters();
            parameters.Add("CollectionId", collectionId);

            var sql =
                "SELECT count(oc.Ukprn) FROM[dbo].[OrganisationCollection] oc inner join Collection c on oc.CollectionId = c.CollectionId inner join CollectionType ct on c.CollectionTypeId = ct.CollectionTypeId WHERE ct.Type = 'FC' and oc.CollectionId = @CollectionId";

            using (var connection = new SqlConnection(_jobQueueManagerSettings.ConnectionString))
            {
                await connection.OpenAsync(cancellationToken);

                return await connection.QuerySingleOrDefaultAsync<int>(sql, parameters);
            }
        }

        public async Task<List<int>> GetApplicableCollectionYearsByTypeAsync(string collectionType, DateTime dateTime, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context
                    .Collection
                    .Where(x =>
                        x.CollectionType.Type == collectionType
                        && x.ReturnPeriod.Any(r => r.StartDateTimeUtc <= dateTime.Date)
                        && x.ReturnPeriod.Any(r => r.EndDateTimeUtc >= dateTime.Date))
                    .Select(x => x.CollectionId)
                    .ToListAsync(cancellationToken);
            }
        }

        private Data.Entities.ReturnPeriod GetCurrentLastOrNextPeriod(DateTime now, IEnumerable<Data.Entities.ReturnPeriod> collectionReturnPeriods)
        {
            var current =
                collectionReturnPeriods.FirstOrDefault(c => now >= c.StartDateTimeUtc && now <= c.EndDateTimeUtc);

            if (current != null)
            {
                return current;
            }
            else
            {
                var nextPeriod = collectionReturnPeriods
                    .Where(w => w.StartDateTimeUtc > now)
                    .OrderBy(o => o.StartDateTimeUtc)
                    .FirstOrDefault();

                if (nextPeriod != null)
                {
                    return nextPeriod;
                }

                return collectionReturnPeriods.OrderByDescending(p => p.PeriodNumber).First();
            }
        }

        private bool IsCollectionOpen(DateTime now, Data.Entities.ReturnPeriod returnPeriod)
        {
            return returnPeriod.StartDateTimeUtc <= now && now <= returnPeriod.EndDateTimeUtc;
        }

        private async Task<Func<IJobQueueDataContext, Task<SetOverrideDTO>>> ProvideSetOverrideDTOFunc(int collectionID)
        {
            return
                async c => await c.Collection
                    .Select(s => new SetOverrideDTO()
                    {
                        CollectionID = s.CollectionId,
                        ProcessingOverrideFlag = s.ProcessingOverrideFlag
                    }).SingleOrDefaultAsync(s => s.CollectionID == collectionID);
        }
    }
}
