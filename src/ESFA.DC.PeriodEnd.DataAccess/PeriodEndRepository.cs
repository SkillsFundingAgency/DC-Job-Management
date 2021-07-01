using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.Audit.Models.DTOs.PeriodEnd;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Utils;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.PeriodEnd.DataAccess
{
    public sealed class PeriodEndRepository : IPeriodEndRepository
    {
        private readonly ILogger _logger;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly Func<IJobQueueDataContext> _periodEndFactory;
        private readonly PeriodEndJobSettings _periodEndJobSettings;
        private readonly IAuditFactory _auditFactory;

        public PeriodEndRepository(
            ILogger logger,
            IDateTimeProvider dateTimeProvider,
            Func<IJobQueueDataContext> contextFactory,
            PeriodEndJobSettings periodEndJobSettings,
            IAuditFactory auditFactory)
        {
            _logger = logger;
            _dateTimeProvider = dateTimeProvider;
            _periodEndFactory = contextFactory;
            _periodEndJobSettings = periodEndJobSettings;
            _auditFactory = auditFactory;
        }

        public async Task<PeriodEndState> GetStateAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var pathState = await context.PeriodEnd
                    .Where(p => p.Period.PeriodNumber == periodNumber
                                && (collectionYear == 0 || p.Period.Collection.CollectionYear == collectionYear)
                                && p.Period.Collection.CollectionType.Type == collectionType)
                    .Select(p => new PeriodEndState
                    {
                        PeriodEndStarted = p.PeriodEndStarted != null,
                        PeriodEndFinished = p.PeriodEndFinished != null,
                        ProviderReportsPublished = p.ProviderReportsPublished,
                        Fm36ReportsPublished = p.Fm36reportsPublished,
                        McaReportsPublished = p.McareportsPublished,
                        CollectionClosedEmailSent = p.CollectionClosedEmailSent,
                        McaReportsReady = p.McareportsReady,
                        ProviderReportsReady = p.ProviderReportsReady,
                        Fm36ReportsReady = p.Fm36reportsReady,
                        AppsSummarisationFinished = p.AppsSummarisationFinished,
                        DcSummarisationFinished = p.DcSummarisationFinished,
                        EsfSummarisationFinished = p.EsfSummarisationFinished,
                        IsInitialised = true
                    }).SingleOrDefaultAsync();

                if (pathState != null)
                {
                    var referenceDataJobsPaused = await context.Schedule
                        .Where(sch => PeriodEndConstants.ReferenceDataJobsToHold.Contains(sch.Collection.Name))
                        .AllAsync(sch => sch.Paused);
                    pathState.ReferenceDataJobsPaused = referenceDataJobsPaused;
                }

                return pathState;
            }
        }

        public async Task<PeriodEndJobState> GetStateForPathIdAsync(int pathId, int collectionYear, int periodNumber, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var pathState = await context.Path
                    .Where(p => p.HubPathId == pathId
                                && p.PeriodEnd.Period.PeriodNumber == periodNumber
                                && (collectionYear == 0 || p.PeriodEnd.Period.Collection.CollectionYear == collectionYear))
                    .Select(p => new PeriodEndJobState
                    {
                        Period = p.PeriodEnd.Period.PeriodNumber,
                        Position = p.PathItem.Count,
                        PathId = p.HubPathId,
                        Year = p.PeriodEnd.Period.Collection.CollectionYear,
                        CollectionName = p.PeriodEnd.Period.Collection.Name,
                        IsBusy = p.IsBusy
                    }).SingleOrDefaultAsync();

                return pathState;
            }
        }

        public async Task<IEnumerable<PeriodEndJobState>> GetStateForPathsAsync(int collectionYear, int periodNumber, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var pathStates = await context.Path
                    .Where(p => p.PeriodEnd.Period.PeriodNumber == periodNumber
                                && (collectionYear == 0 || p.PeriodEnd.Period.Collection.CollectionYear == collectionYear))
                    .Select(p => new PeriodEndJobState
                    {
                        Period = p.PeriodEnd.Period.PeriodNumber,
                        Position = p.PathItem.Count,
                        PathId = p.HubPathId,
                        Year = p.PeriodEnd.Period.Collection.CollectionYear,
                        CollectionName = p.PeriodEnd.Period.Collection.Name,
                        IsBusy = p.IsBusy
                    }).ToListAsync();

                return pathStates;
            }
        }

        public async Task<PathYearPeriod> GetPathforJobAsync(long jobId, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                return await context.PathItemJob
                    .Where(p => p.JobId == jobId)
                    .Select(p => new PathYearPeriod
                    {
                        PathId = p.PathItem.Path.PathId,
                        HubPathId = p.PathItem.Path.HubPathId,
                        Period = p.PathItem.Path.PeriodEnd.Period.PeriodNumber,
                        Year = p.PathItem.Path.PeriodEnd.Period.Collection.CollectionYear
                    })
                    .SingleAsync();
            }
        }

        public async Task<IEnumerable<PathItemJobModel>> GetPathItemJobStatesAsync(int pathId, int collectionYear, int period, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                return await context.PathItemJob
                    .Where(pij => pij.PathItem.Path.HubPathId == pathId &&
                        pij.PathItem.Path.PeriodEnd.Period.PeriodNumber == period &&
                        (collectionYear == 0 || pij.PathItem.Path.PeriodEnd.Period.Collection.CollectionYear == collectionYear))
                    .Select(pij => new PathItemJobModel
                    {
                        JobId = pij.JobId,
                        Status = pij.Job.Status
                    }).ToListAsync();
            }
        }

        public async Task SavePathItemAsync(PathItemModel pathItemModel, int collectionYear, int period, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                Path path;
                try
                {
                    path = await context.Path
                        .SingleAsync(p => p.HubPathId == pathItemModel.PathId
                                                   && p.PeriodEnd.Period.PeriodNumber == period
                                                   && (collectionYear == 0 || p.PeriodEnd.Period.Collection.CollectionYear == collectionYear));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unable to find path with HubPathId {pathItemModel.PathId} and period number {period} in PeriodEndRepository.SavePathItem", ex);
                    throw;
                }

                var pathItem = await context.PathItem.AddAsync(new PathItem
                {
                    PathId = path.PathId,
                    Ordinal = pathItemModel.Ordinal,
                    PathItemLabel = pathItemModel.Name,
                    IsPausing = pathItemModel.IsPausing,
                    HasJobs = pathItemModel.HasJobs,
                    Hidden = pathItemModel.Hidden
                });
                _logger.LogDebug($"Adding pathItem ordinal {pathItem.Entity.Ordinal} to path {path.PathId}");

                if (pathItemModel.PathItemJobs != null)
                {
                    foreach (var itemJob in pathItemModel.PathItemJobs)
                    {
                        await context.PathItemJob
                            .AddAsync(new PathItemJob
                            {
                                JobId = itemJob.JobId,
                                PathItemId = pathItem.Entity.PathItemId
                            });
                        _logger.LogDebug($"Adding pathItem job {itemJob.JobId} to path {path.PathId}");
                    }
                }

                try
                {
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    throw;
                }
            }
        }

        public async Task<bool> SavePathIsBusyAsync(int pathId, bool isBusy, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var data = await context.FromSqlAsync<int>(
                    CommandType.StoredProcedure,
                    "dbo.SetPathIsBusy",
                    new { pathId, isBusy });

                bool hasSaved = data?.First() == 1;

                if (!hasSaved)
                {
                    _logger.LogWarning("Save failed. Path.isBusy flag may have been changed by another process. Please reload and try save again");
                }

                return hasSaved;
            }
        }

        public async Task<IEnumerable<PathModel>> GetPathsForPeriodAsync(
            int collectionYear,
            int periodNumber,
            string collectionType,
            CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                try
                {
                    return await context.Path
                        .Where(p => p.PeriodEnd.Period.PeriodNumber == periodNumber
                                    && (collectionYear == 0 ||
                                        p.PeriodEnd.Period.Collection.CollectionYear == collectionYear)
                                    && p.PeriodEnd.Period.Collection.CollectionType.Type == collectionType)
                        .OrderBy(p => p.Ordinal)
                        .Select(p => new PathModel
                        {
                            PathId = p.PathId,
                            HubPathId = p.HubPathId,
                            Ordinal = p.Ordinal,
                            PathLabel = p.PathLabel
                        })
                        .ToListAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    throw;
                }
            }
        }

        public async Task<IEnumerable<PathItemModel>> GetPathItemsForPathAsync(int pathId, int collectionYear, int periodNumber, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                try
                {
                    return await context.PathItem
                        .Where(pi => pi.Path.PathId == pathId
                                     && pi.Path.PeriodEnd.Period.PeriodNumber == periodNumber
                                     && (collectionYear == 0 || pi.Path.PeriodEnd.Period.Collection.CollectionYear == collectionYear))
                        .OrderBy(pi => pi.Ordinal)
                        .Select(pi => new PathItemModel
                        {
                            PathId = pi.Path.HubPathId,
                            IsPausing = pi.IsPausing ?? false,
                            Name = pi.PathItemLabel,
                            HasJobs = pi.HasJobs ?? false,
                            Hidden = pi.Hidden,
                            Ordinal = pi.Ordinal,
                            PathItemJobs = pi.PathItemJob
                                .Select(pij => new PathItemJobModel
                                {
                                    JobId = pij.JobId,
                                    Status = pij.Job.Status
                                }).ToList()
                        })
                        .ToListAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    throw;
                }
            }
        }

        public async Task<Dictionary<int, PathItemJobsWithSummaries>> GetPathItemJobStatesWithSummaryAsync(
            int collectionYear, int period, CancellationToken cancellationToken)
        {
            var state = new Dictionary<int, PathItemJobsWithSummaries>();

            try
            {
                using (var connection = new SqlConnection(_periodEndJobSettings.ConnectionString))
                {
                    await connection.OpenAsync();

                    SqlMapper.GridReader results = await connection.QueryMultipleAsync(
                        "[dbo].[GetPeriodEndJobs]",
                        commandType: CommandType.StoredProcedure,
                        param: new
                        {
                            pathId = -1,
                            collectionYear = collectionYear,
                            periodNumber = period
                        });

                    var summaries = (await results.ReadAsync<PathItemJobSummary>()).ToList();
                    var jobs = (await results.ReadAsync<PathItemJobPath>()).ToList();

                    foreach (PathItemJobSummary pathItemJobSummary in summaries)
                    {
                        if (!state.TryGetValue(pathItemJobSummary.HubPathId, out var pathItemJobsWithSummaries))
                        {
                            pathItemJobsWithSummaries = new PathItemJobsWithSummaries();
                            state[pathItemJobSummary.HubPathId] = pathItemJobsWithSummaries;
                        }

                        pathItemJobsWithSummaries.Summaries.Add(pathItemJobSummary);
                    }

                    foreach (PathItemJobPath pathItemJobModel in jobs)
                    {
                        pathItemJobModel.CanRetry = pathItemJobModel.Rank == -1;

                        state[pathItemJobModel.HubPathId].Jobs.Add(pathItemJobModel);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get job data", ex);
            }

            return state;
        }

        public async Task<bool> GetValidityForSubPathAsync(int pathId, int year, int period, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var validForPeriod = context.SubPathValidityPeriod
                    .Where(vp => vp.HubPathId == pathId && vp.CollectionYear == year && vp.Period == period)
                    .Select(vp => vp.Enabled)
                    .FirstOrDefault();

                return validForPeriod ?? false;
            }
        }

        public async Task<IEnumerable<int>> GetValidityPeriodsForEmailAsync(int hubEmailId, CancellationToken cancellationToken)
        {
            List<int> periods;
            using (var context = _periodEndFactory())
            {
                try
                {
                    periods = await context.EmailValidityPeriod
                        .Where(vp =>
                            vp.HubEmailId == hubEmailId && vp.Enabled.HasValue && vp.Enabled.Value)
                        .Select(vp => vp.Period).ToListAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    throw;
                }
            }

            return periods;
        }

        public async Task InitialisePeriodEndAsync(
            int period,
            string collectionName,
            IDictionary<int, string> paths,
            CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                ReturnPeriod returnPeriod;
                try
                {
                    returnPeriod = await context.ReturnPeriod
                        .SingleAsync(rp => rp.PeriodNumber == period && rp.Collection.Name.Equals(collectionName));
                }
                catch (Exception e)
                {
                    _logger.LogError($"Unable to find return period with collection name {collectionName} and periodNumber {period} in PeriodEndRepository.PeriodEndStart", e);
                    throw;
                }

                var periodEnd = new JobQueueManager.Data.Entities.PeriodEnd
                {
                    PeriodId = returnPeriod.ReturnPeriodId
                };

                var updatedPeriodEnd = await context.PeriodEnd.AddAsync(periodEnd);

                var ordinal = 0;
                foreach (var path in paths)
                {
                    await context.Path.AddAsync(new Path
                    {
                        HubPathId = path.Key,
                        Ordinal = ordinal++,
                        PathLabel = path.Value,
                        PeriodEndId = updatedPeriodEnd.Entity.PeriodEndId
                    });
                }

                context.SaveChanges();
            }
        }

        public async Task ToggleReferenceDataJobsAsync(bool pause, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var schedules = await context.Schedule
                    .Where(sch => PeriodEndConstants.ReferenceDataJobsToHold.Contains(sch.Collection.Name))
                    .ToListAsync();

                foreach (var schedule in schedules)
                {
                    schedule.Paused = pause;
                    context.Schedule.Update(schedule);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task StartPeriodEndAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var audit = _auditFactory.BuildDataAudit(
                    await ProvideStartPeriodEndDTOFunc(periodNumber, collectionType, collectionYear), context);

                var periodEnd = await context.PeriodEnd
                    .SingleAsync(pe => pe.Period.PeriodNumber == periodNumber
                                       && (collectionYear == 0 || pe.Period.Collection.CollectionYear == collectionYear)
                                       && pe.Period.Collection.CollectionType.Type == collectionType);

                periodEnd.PeriodEndStarted = _dateTimeProvider.GetNowUtc();

                await context.SaveChangesAsync();

                await audit.AfterAndSaveAsync(cancellationToken);
            }
        }

        public async Task PublishProviderReportsAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var audit = _auditFactory.BuildDataAudit(
                    await ProvidePublishProviderReportsDTOFunc(periodNumber, collectionType, collectionYear), context);
                await audit.BeforeAsync(cancellationToken);
                var periodEnd = await GetPeriodEndAsync(collectionYear, periodNumber, collectionType, context, cancellationToken);

                periodEnd.ProviderReportsPublished = true;

                await context.SaveChangesAsync();
                await audit.AfterAndSaveAsync(cancellationToken);
            }
        }

        public async Task PublishFm36ReportsAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var audit = _auditFactory.BuildDataAudit(
                    await ProvidePublishProviderReportsDTOFunc(periodNumber, collectionType, collectionYear), context);
                await audit.BeforeAsync(cancellationToken);
                var periodEnd = await GetPeriodEndAsync(collectionYear, periodNumber, collectionType, context, cancellationToken);

                periodEnd.Fm36reportsPublished = true;

                await context.SaveChangesAsync();
                await audit.AfterAndSaveAsync(cancellationToken);
            }
        }

        public async Task PublishMcaReportsAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var audit = _auditFactory.BuildDataAudit(
                   await ProvidePublishMCAReportsDTOFunc(periodNumber, collectionType, collectionYear), context);
                await audit.BeforeAsync(cancellationToken);
                var periodEnd = await GetPeriodEndAsync(collectionYear, periodNumber, collectionType, context, cancellationToken);

                periodEnd.McareportsPublished = true;

                await context.SaveChangesAsync();
                await audit.AfterAndSaveAsync(cancellationToken);
            }
        }

        public async Task CollectionClosedEmailSentAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await ProvideClosedCollectionEmailDTOFunc(collectionYear, periodNumber, collectionType), context);
                await audit.BeforeAsync(cancellationToken);
                var periodEnd = await GetPeriodEndAsync(collectionYear, periodNumber, collectionType, context, cancellationToken);

                periodEnd.CollectionClosedEmailSent = true;

                await context.SaveChangesAsync();

                await audit.AfterAndSaveAsync(cancellationToken);
            }
        }

        public async Task McaReportsReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var periodEnd = await GetPeriodEndAsync(collectionYear, periodNumber, collectionType, context, cancellationToken);

                periodEnd.McareportsReady = true;

                await context.SaveChangesAsync();
            }
        }

        public async Task ProviderReportsReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var periodEnd = await GetPeriodEndAsync(collectionYear, periodNumber, collectionType, context, cancellationToken);

                periodEnd.ProviderReportsReady = true;

                await context.SaveChangesAsync();
            }
        }

        public async Task Fm36ReportsReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var periodEnd = await GetPeriodEndAsync(collectionYear, periodNumber, collectionType, context, cancellationToken);

                periodEnd.Fm36reportsReady = true;

                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ReportsPublished>> GetPublishedReportPeriodsAsync(CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var publishedReports = await context.PeriodEnd
                    .Where(pe => pe.ProviderReportsPublished || pe.Fm36reportsPublished || pe.McareportsPublished || pe.FrmReportsPublished)
                    .Select(x => new ReportsPublished
                    {
                        CollectionYear = x.Period.Collection.CollectionYear.GetValueOrDefault(),
                        Period = x.Period.PeriodNumber,
                        ProviderReportsPublished = x.ProviderReportsPublished,
                        Fm36ReportsPublished = x.Fm36reportsPublished,
                        McaReportsPublished = x.McareportsPublished,
                        FrmReportsPublished = x.FrmReportsPublished
                    }).ToListAsync();

                return publishedReports;
            }
        }

        public async Task<List<McaDetails>> GetActiveMcaProvidersAsync(int collectionYear, DateTime periodEndDate, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var details = await context.Mcadetail
                    .Where(m => m.AcademicYearFrom <= collectionYear &&
                                (m.AcademicYearTo == null || m.AcademicYearTo >= collectionYear))
                    .Where(m => m.EffectiveFrom <= periodEndDate &&
                                (m.EffectiveTo == null || m.EffectiveTo >= periodEndDate))
                    .Select(d => new McaDetails
                    {
                        UkPrn = d.Ukprn,
                        Code = d.Glacode
                    }).ToListAsync(cancellationToken);

                return details;
            }
        }

        public async Task ClosePeriodEndAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var audit = _auditFactory.BuildDataAudit(await ProvideClosePeriodEndDTOFunc(periodNumber, collectionType, collectionYear), context);
                await audit.BeforeAsync(cancellationToken);
                var periodEnd = await context.PeriodEnd
                    .SingleAsync(
                        pe => pe.Period.PeriodNumber == periodNumber
                              && (collectionYear == 0 || pe.Period.Collection.CollectionYear == collectionYear)
                              && pe.Period.Collection.CollectionType.Type == collectionType,
                        cancellationToken);

                periodEnd.Closed = true;
                periodEnd.PeriodEndFinished = _dateTimeProvider.GetNowUtc();

                await context.SaveChangesAsync(cancellationToken);
                await audit.AfterAndSaveAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<HistoryDetail>> GetPeriodEndHistoryDetailsAsync(string collectionName, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _periodEndFactory())
                {
                    var result = await context.PeriodEnd
                        .Where(pe => pe.Period.Collection.Name == collectionName)
                        .Select(pe => new HistoryDetail
                        {
                            Period = pe.Period.PeriodNumber,
                            PeriodEndId = pe.PeriodEndId,
                            Year = pe.Period.Collection.CollectionYear.GetValueOrDefault(),
                            PeriodEndStart = pe.PeriodEndStarted == null ? pe.PeriodEndStarted : _dateTimeProvider.ConvertUtcToUk(pe.PeriodEndStarted.Value),
                            PeriodEndFinish = pe.PeriodEndFinished == null ? pe.PeriodEndFinished : _dateTimeProvider.ConvertUtcToUk(pe.PeriodEndFinished.Value)
                        })
                        .OrderByDescending(hd => hd.Period)
                        .ToListAsync(cancellationToken);
                    return result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task<IEnumerable<int>> GetCollectionYearsAsync(string collectionNamePrefix, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _periodEndFactory())
                {
                    var result = await context.ReturnPeriod
                        .Where(rp => rp.Collection.Name.Contains(collectionNamePrefix))
                        .Select(rp => rp.Collection.CollectionYear.Value)
                        .Distinct()
                        .ToListAsync();

                    var years = result.OrderBy(y => y);

                    return years;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        public async Task<IDictionary<string, IEnumerable<int>>> GetValidPeriodsForCollectionsAndEmailsAsync(int collectionYear, CancellationToken cancellationToken)
        {
            Dictionary<string, IEnumerable<int>> periodsAndCollections;
            using (var context = _periodEndFactory())
            {
                try
                {
                    periodsAndCollections = await context.ValidityPeriod
                        .Where(vp => vp.Enabled.HasValue
                                        && vp.Enabled.Value
                                        && vp.CollectionYear == collectionYear)
                        .GroupBy(x => $"{x.HubPathItemId}-{PeriodEndEntityType.PathItem}")
                        .ToDictionaryAsync(key => key.Key, value => value.Select(x => x.Period), cancellationToken);

                    var emails = await context.EmailValidityPeriod
                        .Where(vp => vp.Enabled.HasValue
                                            && vp.Enabled.Value
                                            && vp.CollectionYear == collectionYear)
                        .GroupBy(x => $"{x.HubEmailId}-{PeriodEndEntityType.Email}")
                        .ToDictionaryAsync(key => key.Key.ToString(), value => value.Select(x => x.Period), cancellationToken);

                    foreach (KeyValuePair<string, IEnumerable<int>> keyValuePair in emails)
                    {
                        periodsAndCollections.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    throw;
                }
            }

            return periodsAndCollections;
        }

        public async Task EsfSummarisationReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var periodEnd = await GetPeriodEndAsync(collectionYear, periodNumber, collectionType, context, cancellationToken);

                periodEnd.EsfSummarisationFinished = _dateTimeProvider.GetNowUtc();

                await context.SaveChangesAsync();
            }
        }

        public async Task DcSummarisationReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var periodEnd = await GetPeriodEndAsync(collectionYear, periodNumber, collectionType, context, cancellationToken);

                periodEnd.DcSummarisationFinished = _dateTimeProvider.GetNowUtc();

                await context.SaveChangesAsync();
            }
        }

        public async Task AppsSummarisationReadyAsync(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var periodEnd = await GetPeriodEndAsync(collectionYear, periodNumber, collectionType, context, cancellationToken);

                periodEnd.AppsSummarisationFinished = _dateTimeProvider.GetNowUtc();

                await context.SaveChangesAsync();
            }
        }

        public async Task<int> GetPeriodEndJobsForWeekendDaysAsync(int periodEndId, List<DateTime> weekendDays, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                return await (from a in context.Job
                              join b in context.PathItemJob on a.JobId equals b.JobId
                              join c in context.PathItem on b.PathItemId equals c.PathItemId
                              join d in context.Path on c.PathId equals d.PathId
                              join e in context.PeriodEnd on d.PeriodEndId equals e.PeriodEndId
                              where e.PeriodEndId == periodEndId
                              where weekendDays.Contains(a.DateTimeCreatedUtc.Date)
                              group a by a.DateTimeCreatedUtc.Date into f
                              select f.Key)
                                       .CountAsync(cancellationToken);
            }
        }

        public async Task<bool> HasPeriodEndRunForPeriodAsync(string collectionType, int collectionYear, int period, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                return await context.PeriodEnd
                    .AnyAsync(
                        pe => pe.Period.Collection.CollectionType.Type == collectionType
                               && pe.Period.Collection.CollectionYear == collectionYear
                               && pe.Period.PeriodNumber == period
                               && pe.Closed,
                        cancellationToken);
            }
        }

        public async Task<bool> IsPeriodEndRunning(int collectionYear, int periodNumber, string collectionType, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                var periodEnd = await context.PeriodEnd
                    .SingleOrDefaultAsync(
                        pe => pe.Period.PeriodNumber == periodNumber
                              && pe.Period.Collection.CollectionYear == collectionYear
                              && pe.Period.Collection.CollectionType.Type == collectionType,
                        cancellationToken);

                return periodEnd?.PeriodEndStarted != null && periodEnd?.PeriodEndFinished == null;
            }
        }

        public async Task<Dictionary<int, bool>> PathItemsHaveJobsAsync(int pathId, int periodNumber, int collectionYear, CancellationToken cancellationToken)
        {
            using (var context = _periodEndFactory())
            {
                return await context.PathItem.Where(pi => pi.Path.HubPathId == pathId
                            && pi.Path.PeriodEnd.Period.PeriodNumber == periodNumber
                            && (collectionYear == 0 || pi.Path.PeriodEnd.Period.Collection.CollectionYear == collectionYear))
                    .ToDictionaryAsync(pi => pi.Ordinal, pi => pi.HasJobs ?? false, cancellationToken);
            }
        }

        private async Task<JobQueueManager.Data.Entities.PeriodEnd> GetPeriodEndAsync(int collectionYear, int periodNumber, string collectionType, IJobQueueDataContext context, CancellationToken cancellationToken)
        {
            return await context.PeriodEnd
                .SingleAsync(
                    pe => pe.Period.PeriodNumber == periodNumber
                            && pe.Period.Collection.CollectionYear == collectionYear
                            && pe.Period.Collection.CollectionType.Type == collectionType,
                    cancellationToken);
        }

        private async Task<Func<IJobQueueDataContext, Task<ClosedCollectionEmailDTO>>> ProvideClosedCollectionEmailDTOFunc(int collectionYear, int periodNumber, string collectionType)
        {
            return
                async c => await c.PeriodEnd
                    .Select(s => new ClosedCollectionEmailDTO()
                    {
                        CollectionType = s.Period.Collection.CollectionType.Type,
                        Year = s.Period.Collection.CollectionYear,
                        Period = s.Period.PeriodNumber,
                        EmailSent = s.CollectionClosedEmailSent
                    }).SingleOrDefaultAsync(s => s.Period == periodNumber && s.CollectionType == collectionType && s.Year == collectionYear);
        }

        private async Task<Func<IJobQueueDataContext, Task<ClosePeriodEndDTO>>> ProvideClosePeriodEndDTOFunc(int periodNumber, string collectionType, int? collectionYear)
        {
            return
                async c => await c.PeriodEnd
                    .Select(s => new ClosePeriodEndDTO()
                    {
                        CollectionType = s.Period.Collection.CollectionType.Type,
                        CollectionYear = s.Period.Collection.CollectionYear,
                        Period = s.Period.PeriodNumber,
                        IsClosed = s.Closed
                    }).SingleOrDefaultAsync(s => s.Period == periodNumber && s.CollectionType == collectionType && s.CollectionYear == collectionYear);
        }

        private async Task<Func<IJobQueueDataContext, Task<StartPeriodEndDTO>>> ProvideStartPeriodEndDTOFunc(int periodNumber, string collectionType, int? collectionYear)
        {
            return
                async c => await c.PeriodEnd
                    .Select(s => new StartPeriodEndDTO()
                    {
                        CollectionType = s.Period.Collection.CollectionType.Type,
                        CollectionYear = s.Period.Collection.CollectionYear,
                        Period = s.Period.PeriodNumber
                    }).SingleOrDefaultAsync(s => s.Period == periodNumber && s.CollectionType == collectionType && s.CollectionYear == collectionYear);
        }

        private async Task<Func<IJobQueueDataContext, Task<PublishMCAReportsDTO>>> ProvidePublishMCAReportsDTOFunc(int periodNumber, string collectionType, int? collectionYear)
        {
            return
                async c => await c.PeriodEnd
                    .Select(s => new PublishMCAReportsDTO()
                    {
                        Period = s.Period.PeriodNumber,
                        Year = s.Period.Collection.CollectionYear,
                        CollectionType = s.Period.Collection.CollectionType.Type,
                        IsPublished = s.McareportsPublished
                    }).SingleOrDefaultAsync(s => s.Year == collectionYear && s.Period == periodNumber && s.CollectionType == collectionType);
        }

        private async Task<Func<IJobQueueDataContext, Task<PublishProviderReportsDTO>>> ProvidePublishProviderReportsDTOFunc(int periodNumber, string collectionType, int? collectionYear)
        {
            return
                async c => await c.PeriodEnd
                    .Select(s => new PublishProviderReportsDTO()
                    {
                        Period = s.Period.PeriodNumber,
                        CollectionYear = s.Period.Collection.CollectionYear,
                        CollectionType = s.Period.Collection.CollectionType.Type,
                        IsPublished = s.ProviderReportsPublished
                    }).SingleOrDefaultAsync(s => s.CollectionYear == collectionYear && s.Period == periodNumber && s.CollectionType == collectionType);
        }
    }
}