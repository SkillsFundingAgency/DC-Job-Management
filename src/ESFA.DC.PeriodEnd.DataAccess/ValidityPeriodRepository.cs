using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.PathItems;
using ESFA.DC.PeriodEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.PeriodEnd.DataAccess
{
    public class ValidityPeriodRepository : IValidityPeriodRepository
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly List<IILRPathItem> _pathItems;

        public ValidityPeriodRepository(
            Func<IJobQueueDataContext> contextFactory,
            IEnumerable<IILRPathItem> pathItems,
            IDASStoppingPathItem dasStop)
        {
            _contextFactory = contextFactory;
            _pathItems = pathItems.ToList();

            // Add additional non IILRPathItem path related items
            _pathItems.Add(dasStop);
        }

        public async Task<List<ValidityPeriodLookupModel>> GetValidityPeriodList(CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.ValidityPeriod
                    .Select(x => new ValidityPeriodLookupModel
                    {
                        PathItemId = x.HubPathItemId,
                        CollectionYear = x.CollectionYear,
                        Period = x.Period,
                        Enabled = x.Enabled
                    }).ToListAsync(cancellationToken);
            }
        }

        public async Task<ValidityPeriodLookupModel> GetValidityPeriod(int collectionId, int collectionYear, int period, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await GetValidityPeriods(context.ValidityPeriod, x => x.Period == period && x.CollectionYear == collectionYear)
                    .Select(vp => new ValidityPeriodLookupModel
                    {
                        PathItemId = vp.HubPathItemId,
                        CollectionYear = vp.CollectionYear,
                        Period = vp.Period,
                        Enabled = vp.Enabled
                    })
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        public async Task<int> UpdateValidityPeriod(ValidityPeriodLookupModel validityPeriodLookupModel, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var entity = await GetValidityPeriods(
                    context.ValidityPeriod,
                    x => x.HubPathItemId == validityPeriodLookupModel.PathItemId
                         && x.Period == validityPeriodLookupModel.Period
                         && x.CollectionYear == validityPeriodLookupModel.CollectionYear)
                    .FirstOrDefaultAsync(cancellationToken);

                if (entity != null)
                {
                    entity.Enabled = validityPeriodLookupModel.Enabled;
                    return await context.SaveChangesAsync(cancellationToken);
                }
            }

            return 0;
        }

        public async Task<List<ValidityPeriodLookupModel>> GetValidityPeriodList(int collectionYear, int period, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var validityPeriodEntities =
                    await GetValidityPeriods(context.ValidityPeriod, x => x.Period == period && x.CollectionYear == collectionYear)
                    .Select(x => new { x.CollectionYear, x.Period, x.Enabled, x.HubPathItemId })
                    .ToListAsync(cancellationToken);

                var validityPeriodLookupModels = new List<ValidityPeriodLookupModel>();
                var pathItems = _pathItems
                    .Where(pi => !pi.IsHidden) // exclude hidden/fake path items from the ui list
                    .Select(x => new { x.DisplayName, x.PathItemId })
                    .ToList();

                foreach (var pathItem in pathItems)
                {
                    var validityPeriodEntity = validityPeriodEntities
                        .FirstOrDefault(x => x.HubPathItemId == pathItem.PathItemId && x.CollectionYear == collectionYear && x.Period == period);

                    if (validityPeriodEntity != null)
                    {
                        validityPeriodLookupModels.Add(new ValidityPeriodLookupModel
                        {
                            CollectionYear = validityPeriodEntity.CollectionYear,
                            Period = validityPeriodEntity.Period,
                            Enabled = validityPeriodEntity.Enabled,
                            PathItemId = pathItem.PathItemId
                        });
                    }
                }

                return validityPeriodLookupModels;
            }
        }

        public async Task<Dictionary<int, ValidityPeriodLookupModel>> GetEmailValidities(int collectionYear, int period, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.EmailValidityPeriod
                    .Where(vp => vp.CollectionYear == collectionYear && vp.Period == period)
                    .ToDictionaryAsync(
                        vp => vp.HubPathItemId,
                        vp => new ValidityPeriodLookupModel
                        {
                            CollectionYear = collectionYear,
                            Period = period,
                            Enabled = vp.Enabled
                        },
                        cancellationToken);
            }
        }

        public async Task<Dictionary<int, SubPathValidityPeriodLookupModel>> GetPathValidityPeriods(int collectionYear, int period, CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                return await context.SubPathValidityPeriod
                    .Where(vp => vp.CollectionYear == collectionYear && vp.Period == period)
                    .ToDictionaryAsync(
                        vp => vp.HubPathId,
                        vp => new SubPathValidityPeriodLookupModel
                        {
                            PathId = vp.HubPathId,
                            CollectionYear = collectionYear,
                            Period = period,
                            Enabled = vp.Enabled
                        },
                        cancellationToken);
            }
        }

        public async Task UpdateSubPathValidityPeriods(
            List<SubPathValidityPeriodLookupModel> subPathValidityPeriods,
            CancellationToken cancellationToken)
        {
            using (var context = _contextFactory())
            {
                var paths = subPathValidityPeriods.Select(vp => vp.PathId).Distinct();

                var entities = await context.SubPathValidityPeriod
                    .Where(x => paths.Contains(x.HubPathId))
                    .ToListAsync(cancellationToken);

                foreach (var x in entities)
                {
                    var valitityPeriodLookupModel = subPathValidityPeriods
                        .FirstOrDefault(vp => vp.PathId == x.HubPathId && vp.Period == x.Period && vp.CollectionYear == x.CollectionYear);

                    if (valitityPeriodLookupModel != null)
                    {
                        x.Enabled = valitityPeriodLookupModel.Enabled;
                    }
                }

                await context.SaveChangesAsync(cancellationToken);
            }
        }

        private IQueryable<ValidityPeriod> GetValidityPeriods(IQueryable<ValidityPeriod> validityPeriods, Expression<Func<ValidityPeriod, bool>> exp)
        {
            return validityPeriods.Where(exp).OrderBy(x => x.HubPathItemId).ThenBy(x => x.Period);
        }
    }
}