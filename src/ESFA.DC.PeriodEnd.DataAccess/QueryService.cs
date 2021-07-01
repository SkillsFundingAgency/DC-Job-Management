using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Audit.Models.DTOs.PeriodEnd;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.PeriodEnd.DataAccess
{
    public class QueryService : IQueryService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly ILogger _logger;
        private readonly IAuditFactory _auditFactory;
        private readonly IJsonSerializationService _jsonSerializationService;

        public QueryService(
            Func<IJobQueueDataContext> contextFactory,
            ILogger logger,
            IAuditFactory auditFactory,
            IJsonSerializationService jsonSerializationService)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _auditFactory = auditFactory;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<IEnumerable<ProviderJob>> GetNonSubmittingProviders(string collectionName, int periodNumber)
        {
            List<ProviderJob> providerJobs;

            try
            {
                using (var context = _contextFactory())
                {
                    providerJobs = (await context
                        .FromSqlAsync<ProviderJob>(
                            CommandType.StoredProcedure,
                            "GetNonSubmittingProviders",
                            new { collectionName, periodNumber }))
                        .ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting non-submitting providers for collection {collectionName} and period {periodNumber}", e);
                throw;
            }

            return providerJobs;
        }

        public async Task<IEnumerable<ProviderJob>> GetLatestSubmittedJobs(string collectionName)
        {
            List<ProviderJob> providerJobs;

            try
            {
                using (var context = _contextFactory())
                {
                    providerJobs = (await context
                            .FromSqlAsync<ProviderJob>(
                                CommandType.StoredProcedure,
                                "GetLatestSubmittedJobs",
                                new { collectionName }))
                        .ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting latest submitted jobs for collection {collectionName}", e);
                throw;
            }

            return providerJobs;
        }

        public async Task<IEnumerable<ProviderJob>> GetLatestDASSubmittedJobs(string ilrCollectionName, string dasSubmissionCollectionName)
        {
            List<ProviderJob> providerJobs;

            try
            {
                using (var context = _contextFactory())
                {
                    providerJobs = (await context
                            .FromSqlAsync<ProviderJob>(
                                CommandType.StoredProcedure,
                                "GetLatestDASSubmittedJobs",
                                new { ilrCollectionName, dasSubmissionCollectionName }))
                        .ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting latest submitted jobs for ilr collection {ilrCollectionName} and DAS collection {dasSubmissionCollectionName}", e);
                throw;
            }

            return providerJobs;
        }

        public async Task<IEnumerable<ProviderJob>> GetSubmittingProviders(string collectionName)
        {
            List<ProviderJob> providerJobs;

            try
            {
                using (var context = _contextFactory())
                {
                    providerJobs = (await context
                            .FromSqlAsync<ProviderJob>(
                                CommandType.StoredProcedure,
                                "GetSubmittingProviders",
                                new { collectionName }))
                        .ToList();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error getting submitting providers for collection {collectionName} ", e);
                throw;
            }

            return providerJobs;
        }

        public async Task ClearDownPeriodEnd(string collectionName, int period)
        {
            using (var context = _contextFactory())
            {
                try
                {
                    await context
                        .FromSqlAsync<ProviderJob>(
                            CommandType.StoredProcedure,
                            "PeriodEnd.PeriodEndClearDown",
                            new { period, collectionName });
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    throw;
                }
            }
        }

        public async Task UpdateValidityPeriods(int collectionYear, int period, List<ValidityPeriodModel> validities)
        {
            var validityPeriods = _jsonSerializationService.Serialize(validities);
            var auditList = new List<IAudit>();

            using (var context = _contextFactory())
            {
                try
                {
                    var subPathValidityAudit = _auditFactory.BuildDataAudit(await ProvideSaveValidityPeriodDTOFuncSubpath(collectionYear, period, 1, validities.Where(x => x.EntityType == 1).ToList()), context);
                    await subPathValidityAudit.BeforeAsync(CancellationToken.None);
                    auditList.Add(subPathValidityAudit);
                    var validityAudit = _auditFactory.BuildDataAudit(await ProvideSaveValidityPeriodDTOFuncValidity(collectionYear, period, 2, validities.Where(x => x.EntityType == 2).ToList()), context);
                    await validityAudit.BeforeAsync(CancellationToken.None);
                    auditList.Add(validityAudit);
                    var emailValidityAudit = _auditFactory.BuildDataAudit(await ProvideSaveValidityPeriodDTOFuncEmail(collectionYear, period, 3, validities.Where(x => x.EntityType == 3).ToList()), context);
                    await emailValidityAudit.BeforeAsync(CancellationToken.None);
                    auditList.Add(emailValidityAudit);

                    await context
                        .FromSqlAsync<ProviderJob>(
                            CommandType.StoredProcedure,
                            "UpdateValidityPeriods",
                            new { collectionYear, period, validityPeriods });
                    foreach (var audit in auditList)
                    {
                        await audit.AfterAndSaveAsync(CancellationToken.None);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    throw;
                }
            }
        }

        private async Task<Func<IJobQueueDataContext, Task<List<SaveValidityChangesDTO>>>>
            ProvideSaveValidityPeriodDTOFuncSubpath(int collectionYear, int period, int entityType, List<ValidityPeriodModel> changesBeingMade)
        {
            return
                async c =>
                {
                    var current = await c.SubPathValidityPeriod
                        .Where(v => v.CollectionYear == collectionYear && v.Period == period && changesBeingMade.Any(y => y.Id == v.HubPathId && y.Enabled == !v.Enabled && y.EntityType == entityType))
                        .Select(s => new SaveValidityChangesDTO()
                        {
                            Period = s.Period,
                            HubPathId = s.HubPathId,
                            CollectionYear = s.CollectionYear,
                            EntityType = entityType,
                            Enabled = s.Enabled
                        }).ToListAsync();
                    changesBeingMade = AfterPreparation(changesBeingMade, current);
                    return current;
                };
        }

        private async Task<Func<IJobQueueDataContext, Task<List<SaveValidityChangesDTO>>>> ProvideSaveValidityPeriodDTOFuncValidity(int collectionYear, int period, int entityType, List<ValidityPeriodModel> changesBeingMade)
        {
            return
                async c =>
                {
                    var current = await c.ValidityPeriod
                        .Where(v => v.CollectionYear == collectionYear && v.Period == period && changesBeingMade.Any(y => y.Id == v.HubPathItemId && y.Enabled == !v.Enabled && y.EntityType == entityType))
                        .Select(s => new SaveValidityChangesDTO()
                        {
                            Period = s.Period,
                            HubPathId = s.HubPathItemId,
                            CollectionYear = s.CollectionYear,
                            EntityType = entityType,
                            Enabled = s.Enabled
                        }).ToListAsync();
                    changesBeingMade = AfterPreparation(changesBeingMade, current);
                    return current;
                };
        }

        private async Task<Func<IJobQueueDataContext, Task<List<SaveValidityChangesDTO>>>> ProvideSaveValidityPeriodDTOFuncEmail(int collectionYear, int period, int entityType, List<ValidityPeriodModel> changesBeingMade)
        {
            return
                async c =>
                {
                    var current = await c.EmailValidityPeriod
                        .Where(v => v.CollectionYear == collectionYear && v.Period == period && changesBeingMade.Any(y => y.Id == v.HubPathItemId && y.Enabled == !v.Enabled && y.EntityType == entityType))
                        .Select(s => new SaveValidityChangesDTO()
                        {
                            Period = s.Period,
                            HubPathId = s.HubPathItemId,
                            CollectionYear = s.CollectionYear,
                            EntityType = entityType,
                            Enabled = s.Enabled
                        }).ToListAsync();
                    changesBeingMade = AfterPreparation(changesBeingMade, current);
                    return current;
                };
        }

        private List<ValidityPeriodModel> AfterPreparation(List<ValidityPeriodModel> changesBeingMade, List<SaveValidityChangesDTO> current)
        {
            foreach (var validityPeriodModel in changesBeingMade.Where(x => current.Any(y => y.EntityType == x.EntityType && y.HubPathId == x.Id)))
            {
                validityPeriodModel.Enabled = !validityPeriodModel.Enabled;
            }

            return changesBeingMade;
        }
    }
}