using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.Api.Common.Utilities.Pagination;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.ReadOnlyEntities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Model;
using ESFA.DC.JobQueueManager.Settings;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Processing.DasMismatch;
using ESFA.DC.Jobs.Model.Processing.Detail;
using ESFA.DC.Jobs.Model.Processing.JobsConcern;
using ESFA.DC.Jobs.Model.Processing.JobsFailedCurrentPeriod;
using ESFA.DC.Jobs.Model.Processing.JobsFailedToday;
using ESFA.DC.Jobs.Model.Processing.JobsProcessing;
using ESFA.DC.Jobs.Model.Processing.JobsQueued;
using ESFA.DC.Jobs.Model.Processing.JobsSlowFile;
using ESFA.DC.Jobs.Model.Processing.JobsSubmitted;
using ESFA.DC.Jobs.Model.Processing.ProvidersReturned;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.Organisations.Model;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace ESFA.DC.JobQueueManager
{
    public class JobProcessingService : IJobProcessingService
    {
        private readonly JobQueueManagerSettings _jobQueueManagerSettings;
        private readonly Func<IOrganisationsContext> _orgContextFactory;
        private readonly Func<IJobQueueDataContext> _jobContextFactory;
        private readonly ILogger _logger;
        private readonly IReturnCalendarService _returnCalendarService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly HashSet<string> _collectionTypes = new HashSet<string>() { CollectionTypeConstants.Ilr, CollectionTypeConstants.Eas, CollectionTypeConstants.Esf, CollectionTypeConstants.NCS, CollectionTypeConstants.FundingClaims };

        public JobProcessingService(
            JobQueueManagerSettings jobQueueManagerSettings,
            Func<IOrganisationsContext> orgContextFactory,
            Func<IJobQueueDataContext> jobContextFactory,
            ILogger logger,
            IReturnCalendarService returnCalendarService,
            IJsonSerializationService jsonSerializationService)
        {
            _jobQueueManagerSettings = jobQueueManagerSettings;
            _orgContextFactory = orgContextFactory;
            _jobContextFactory = jobContextFactory;
            _logger = logger;
            _returnCalendarService = returnCalendarService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<JobsQueuedModel> GetJobsThatAreQueued(DateTime dateTime)
        {
            var modelList = (await GetJobs<ReadOnlyJobQueued>("dbo.GetJobsThatAreQueued", new { NOWUTC = dateTime })).ToList();
            var ukprns = modelList.Select(y => y.Ukprn).Distinct().ToList();
            var providers = await GetProviders(ukprns);
            OrgDetail org;

            var jobsQueuedModel = new JobsQueuedModel
            {
                Jobs = modelList.GroupBy(kvp => kvp.CollectionYear)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Select(x => new JobQueuedLookupModel
                        {
                            JobId = x.JobId,
                            ProviderName = providers.TryGetValue(x.Ukprn, out org) ? org.Name : null,
                            Ukprn = x.Ukprn,
                            TimeInQueueSecond = x.TimeInQueueSecond,
                            CollectionType = x.CollectionType,
                            Status = x.Status,
                            StatusDescription = x.StatusDescription
                        }).OrderByDescending(x => x.TimeInQueueSecond).ToList())
            };

            return jobsQueuedModel;
        }

        public async Task<JobsProcessingModel> GetJobsThatAreProcessing(DateTime dateTime)
        {
            var modelList = (await GetJobs<ReadOnlyJobProcessing>("dbo.GetJobsThatAreProcessing", new { NOWUTC = dateTime })).ToList();
            var ukprns = modelList.Select(y => y.Ukprn).Distinct().ToList();
            var providers = await GetProviders(ukprns);
            OrgDetail org;

            var jobsProcessingModel = new JobsProcessingModel
            {
                Jobs = modelList.GroupBy(kvp => kvp.CollectionYear)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Select(x => new JobProcessingLookupModel
                        {
                            JobId = x.JobId,
                            ProviderName = providers.TryGetValue(x.Ukprn, out org) ? org.Name : null,
                            Ukprn = x.Ukprn,
                            DateDifferSecond = x.DateDifferSecond,
                            TimeTakenSecond = x.TimeTakenSecond,
                            CollectionType = x.CollectionType,
                            Status = x.Status,
                            StatusDescription = x.StatusDescription
                        }).ToList())
            };

            return jobsProcessingModel;
        }

        public async Task<JobsSubmittedModel> GetJobsThatAreSubmitted(DateTime dateTime)
        {
            var modelList = (await GetJobs<ReadOnlyJobSubmitted>("dbo.GetJobsThatAreSubmitted", new { FROMMIDNIGHTUTC = dateTime.Date })).ToList();
            var ukprns = modelList.Select(y => y.Ukprn).Distinct().ToList();
            var providers = await GetProviders(ukprns);
            OrgDetail org;

            var jobsSubmittedModel = new JobsSubmittedModel
            {
                Jobs = modelList.GroupBy(kvp => kvp.CollectionYear)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Select(x => new JobSubmittedLookupModel
                        {
                            JobId = x.JobId,
                            ProviderName = providers.TryGetValue(x.Ukprn, out org) ? org.Name : null,
                            Ukprn = x.Ukprn,
                            CreatedDate = x.CreatedDate,
                            FileName = x.FileName,
                            Status = x.Status,
                            StatusDescription = x.StatusDescription,
                            CollectionType = x.CollectionType
                        }).ToList())
            };

            return jobsSubmittedModel;
        }

        public async Task<JobsFailedTodayModel> GetJobsThatAreFailedToday(DateTime dateTime)
        {
            var modelList = (await GetJobs<ReadOnlyJobFailedToday>("dbo.GetJobsThatAreFailedToday", new { FROMMIDNIGHTUTC = dateTime.Date })).ToList();
            var ukprns = modelList.Select(y => y.Ukprn).Distinct().ToList();
            var providers = await GetProviders(ukprns);
            OrgDetail org;

            var jobsFailedTodayModel = new JobsFailedTodayModel
            {
                Jobs = modelList.GroupBy(kvp => kvp.CollectionYear)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Select(x => new JobFailedTodayLookupModel
                        {
                            JobId = x.JobId,
                            ProviderName = providers.TryGetValue(x.Ukprn, out org) ? org.Name : null,
                            Ukprn = x.Ukprn,
                            FileName = x.FileName,
                            FailedAt = x.FailedAt,
                            ProcessingTimeBeforeFailureSecond = x.ProcessingTimeBeforeFailureSecond,
                            CollectionType = x.CollectionType
                        }).ToList())
            };

            return jobsFailedTodayModel;
        }

        public async Task<JobsSlowFileModel> GetJobsThatAreSlowFile(DateTime dateTime)
        {
            var modelList = (await GetJobs<ReadOnlyJobSlowFile>("dbo.GetJobsThatAreSlowFiles", new { NOWUTC = dateTime })).ToList();
            var ukprns = modelList.Select(y => y.Ukprn).Distinct().ToList();
            var providers = await GetProviders(ukprns);
            OrgDetail org;

            var jobsSlowFileModel = new JobsSlowFileModel
            {
                Jobs = modelList.GroupBy(kvp => kvp.CollectionYear)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Select(x => new JobSlowFileLookupModel
                        {
                            JobId = x.JobId,
                            ProviderName = providers.TryGetValue(x.Ukprn, out org) ? org.Name : null,
                            Ukprn = x.Ukprn,
                            FileName = x.FileName,
                            TimeTakenSecond = x.TimeTakenSecond,
                            AverageTimeSecond = x.AverageTimeSecond,
                            CollectionType = x.CollectionType
                        }).ToList())
            };

            return jobsSlowFileModel;
        }

        public async Task<JobConcernModel> GetJobsThatAreConcern(DateTime dateTime)
        {
            var modelList = (await GetJobs<ReadOnlyJobConcern>("dbo.GetJobsThatAreConcern", new { NOWUTC = dateTime })).ToList();
            var ukprns = modelList.Select(y => y.Ukprn).Distinct().ToList();
            var providers = await GetProviders(ukprns);
            OrgDetail org;

            var jobsConcernModel = new JobConcernModel
            {
                Jobs = modelList
                    .GroupBy(kvp => kvp.CollectionYear)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Select(x => new JobConcernLookupModel
                        {
                            JobId = x.JobId,
                            ProviderName = providers.TryGetValue(x.Ukprn, out org) ? org.Name : null,
                            Ukprn = x.Ukprn,
                            FileName = x.FileName,
                            LastSuccessfulSubmission = x.LastSuccessfulSubmission,
                            PeriodOfLastSuccessfulSubmission = x.PeriodOfLastSuccessfulSubmission,
                            CollectionType = x.CollectionType
                        }).ToList())
            };

            return jobsConcernModel;
        }

        public async Task<JobProcessingDetailModel> GetJobsProcessingCurrentPeriodAsync(short jobStatus, DateTime dateTime, CancellationToken cancellationToken)
        {
            var modelList = (await GetJobs<ReadOnlyJobProcessingDetail>("dbo.GetJobsThatAreProcessingDetailCurrentPeriod", new { JobStatus = jobStatus, NOWUTC = dateTime })).ToList();
            var ukprns = modelList.Select(y => y.Ukprn).Distinct().ToList();
            var providers = await GetProviders(ukprns);
            OrgDetail org;

            var jobProcessingDetailModel = new JobProcessingDetailModel
            {
                Jobs = modelList
                    .GroupBy(kvp => kvp.CollectionYear)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Select(x => new JobProcessingDetailLookupModel()
                        {
                            JobId = x.JobId,
                            ProviderName = providers.TryGetValue(x.Ukprn, out org) ? org.Name : null,
                            Ukprn = x.Ukprn,
                            FileName = x.FileName,
                            ProcessingTimeSeconds = x.ProcessingTimeSeconds,
                            CollectionType = x.CollectionType
                        }).ToList())
            };

            return jobProcessingDetailModel;
        }

        public async Task<JobProcessingDetailModel> GetJobsProcessingCurrentPeriodLast5MinsAsync(short jobStatus, DateTime dateTime, CancellationToken cancellationToken)
        {
            var modelList = (await GetJobs<ReadOnlyJobProcessingDetail>("dbo.GetJobsThatAreProcessingDetailCurrentPeriodLast5Mins", new { JobStatus = jobStatus, NOWUTC = dateTime })).ToList();
            var ukprns = modelList.Select(y => y.Ukprn).Distinct().ToList();
            var providers = await GetProviders(ukprns);
            OrgDetail org;

            var jobProcessingDetailModel = new JobProcessingDetailModel
            {
                Jobs = modelList
                    .GroupBy(kvp => kvp.CollectionYear)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Select(x => new JobProcessingDetailLookupModel()
                        {
                            JobId = x.JobId,
                            ProviderName = providers.TryGetValue(x.Ukprn, out org) ? org.Name : null,
                            Ukprn = x.Ukprn,
                            FileName = x.FileName,
                            ProcessingTimeSeconds = x.ProcessingTimeSeconds,
                            CollectionType = x.CollectionType
                        }).ToList())
            };

            return jobProcessingDetailModel;
        }

        public async Task<JobProcessingDetailModel> GetJobsProcessingCurrentPeriodLastHourAsync(short jobStatus, DateTime dateTime, CancellationToken cancellationToken)
        {
            var modelList = (await GetJobs<ReadOnlyJobProcessingDetail>("dbo.GetJobsThatAreProcessingDetailCurrentPeriodLastHour", new { JobStatus = jobStatus, NOWUTC = dateTime })).ToList();
            var ukprns = modelList.Select(y => y.Ukprn).Distinct().ToList();
            var providers = await GetProviders(ukprns);
            OrgDetail org;

            var jobProcessingDetailModel = new JobProcessingDetailModel
            {
                Jobs = modelList
                    .GroupBy(kvp => kvp.CollectionYear)
                    .ToDictionary(
                        grp => grp.Key,
                        grp => grp.Select(x => new JobProcessingDetailLookupModel()
                        {
                            JobId = x.JobId,
                            ProviderName = providers.TryGetValue(x.Ukprn, out org) ? org.Name : null,
                            Ukprn = x.Ukprn,
                            FileName = x.FileName,
                            ProcessingTimeSeconds = x.ProcessingTimeSeconds,
                            CollectionType = x.CollectionType
                        }).ToList())
            };

            return jobProcessingDetailModel;
        }

        public async Task<DasMismatchModel> GetJobsThatAreADasMismatchAsync(int? collectionYear, CancellationToken cancellationToken)
        {
            var dasMismatchModel = new DasMismatchModel();

            using (var connection = new SqlConnection(_jobQueueManagerSettings.DasPaymentsConnectionString))
            {
                await connection.OpenAsync(cancellationToken);

                using (var context = _jobContextFactory())
                {
                    var successfulIlrUkprnsInPeriod = await context.FromSqlAsync<SuccessfulIlrModel>(
                        CommandType.StoredProcedure,
                        "dbo.GetIlrJobsSuccessfulInPeriod",
                        new { collectionYear });

                    if (successfulIlrUkprnsInPeriod == null || !successfulIlrUkprnsInPeriod.Any())
                    {
                        return new DasMismatchModel();
                    }

                    var currentOpenPeriods = successfulIlrUkprnsInPeriod.DistinctBy(d => new { d.CollectionYear, d.PeriodNumber });
                    foreach (var currentOpenPeriod in currentOpenPeriods)
                    {
                        var dynamicParameters = new DynamicParameters();
                        dynamicParameters.Add("@year", currentOpenPeriod.CollectionYear);
                        dynamicParameters.Add("@period", currentOpenPeriod.PeriodNumber);

                        var differences = new List<long>();
                        var providers = await GetProviders(differences);
                        var ukprnList = $@"{{""Ukprn"": {_jsonSerializationService.Serialize(differences)} }}";

                        var mismatchJobs = await context.FromSqlAsync<SldToDasMismatchModel>(
                            CommandType.StoredProcedure,
                            "dbo.GetSldToDasMismatchDrillDown",
                            new
                            {
                                period = currentOpenPeriod.PeriodNumber,
                                year = currentOpenPeriod.CollectionYear,
                                ukprnList = ukprnList
                            });

                        OrgDetail org;

                        dasMismatchModel.Jobs.Add(currentOpenPeriod.CollectionYear, mismatchJobs.Select(s =>
                            new DasMismatchLookupModel()
                            {
                                JobId = s.JobId,
                                Ukprn = s.Ukprn,
                                SubmissionDate = s.DateTimeSubmittedUtc,
                                FileName = s.Filename,
                                ProviderName = providers.TryGetValue(s.Ukprn, out org) ? org.Name : null,
                            }).ToList());
                    }
                }
            }

            return dasMismatchModel;
        }

        public async Task<JobsFailedCurrentPeriodModel> GetFailedJobsInCurrentPeriodAsync(CancellationToken cancellationToken)
        {
            var jobsFailedCurrentPeriodModel = new JobsFailedCurrentPeriodModel();

            using (var context = _jobContextFactory())
            {
                var jobsFailedCurrentPeriod = await context.FromSqlAsync<JobsFailedCurrentPeriodDrilldownModel>(
                    CommandType.StoredProcedure,
                    "dbo.GetFailedFilesCurrentPeriod",
                    new { });

                var providers = await GetProviders(new HashSet<long>(jobsFailedCurrentPeriod.Select(s => s.Ukprn)));
                OrgDetail org;

                jobsFailedCurrentPeriodModel.Jobs = jobsFailedCurrentPeriod
                    .GroupBy(kvp => kvp.CollectionYear)
                    .ToDictionary(grp => grp.Key, grp => grp.Select(kvp => new JobsFailedCurrentPeriodLookupModel()
                    {
                        JobId = kvp.JobId,
                        Ukprn = kvp.Ukprn,
                        CollectionType = kvp.CollectionType,
                        ProviderName = providers.TryGetValue(kvp.Ukprn, out org) ? org.Name : null,
                        DateTimeOfFailure = kvp.DateTimeOfFailure,
                        FileName = kvp.Filename,
                        ProcessingTimeBeforeFailure = kvp.ProcessingTimeBeforeFailure
                    }).ToList());
            }

            return jobsFailedCurrentPeriodModel;
        }

        public async Task<ProvidersReturnedCurrentPeriodModel> GetProvidersReturnedCurrentPeriodAsync(CancellationToken cancellationToken)
        {
            var providersReturnedCurrentPeriodModel = new ProvidersReturnedCurrentPeriodModel();

            using (var context = _jobContextFactory())
            {
                var providersReturnedCurrentPeriod = await context.FromSqlAsync<ProvidersReturnedCurrentPeriodDrilldownModel>(
                    CommandType.StoredProcedure,
                    "dbo.GetProvidersReturnedCurrentPeriod",
                    new { });

                var providers = await GetProviders(new HashSet<long>(providersReturnedCurrentPeriod.Select(s => s.Ukprn)));
                OrgDetail org;

                providersReturnedCurrentPeriodModel.Jobs = providersReturnedCurrentPeriod
                    .GroupBy(kvp => kvp.CollectionYear)
                    .ToDictionary(grp => grp.Key, grp => grp.Select(kvp => new ProvidersReturnedCurrentPeriodLookupModel()
                    {
                        JobId = kvp.JobId,
                        Ukprn = kvp.Ukprn,
                        ProviderName = providers.TryGetValue(kvp.Ukprn, out org) ? org.Name : null,
                        DateTimeSubmission = kvp.DateTimeSubmission,
                        FileName = kvp.Filename,
                        ProcessingTime = kvp.ProcessingTime
                    }).ToList());
            }

            return providersReturnedCurrentPeriodModel;
        }

        public async Task<PaginatedResult<JobDetails>> GetPaginatedJobsInDetailForCurrentOrClosedPeriod(short jobStatus, int pageSize = int.MaxValue, int pageNumber = 1)
        {
            try
            {
                DateTime startDateTimeUtc;
                DateTime endDateTimeUtc;

                var targetPeriods = await _returnCalendarService.GetOpenPeriodsAsync();
                if (!targetPeriods.Any())
                {
                    var closedReturnPeriod = await _returnCalendarService.GetRecentlyClosedPeriodAsync();
                    startDateTimeUtc = closedReturnPeriod.StartDateTimeUtc;
                    endDateTimeUtc = closedReturnPeriod.EndDateTimeUtc;
                }
                else
                {
                    startDateTimeUtc = targetPeriods.Min(x => x.StartDateTimeUtc);
                    endDateTimeUtc = targetPeriods.Max(x => x.EndDateTimeUtc);
                }

                return await GetPaginatedJobsInDetail(jobStatus, startDateTimeUtc, endDateTimeUtc, pageSize, pageNumber);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error occured trying to get job details for period with status : {jobStatus}", exception);
                throw;
            }
        }

        public async Task<PaginatedResult<JobDetails>> GetPaginatedJobsInDetail(
                                    short jobStatus,
                                    DateTime startDateTimeUtc,
                                    DateTime endDateTimeUtc,
                                    int pageSize = int.MaxValue,
                                    int pageNumber = 1)
        {
            try
            {
                PaginatedResult<JobDetails> result;

                using (var context = _jobContextFactory())
                {
                    var queryable = context.ReadOnlyJob
                        .Where(x => x.Status == jobStatus &&
                                    _collectionTypes.Contains(x.CollectionType) &&
                                    x.DateTimeUpdatedUtc >= startDateTimeUtc &&
                                    x.DateTimeUpdatedUtc <= endDateTimeUtc)
                        .Select(j => new JobDetails()
                        {
                            Ukprn = j.Ukprn,
                            CollectionName = j.CollectionName,
                            CollectionYear = j.CollectionYear,
                            FileName = j.FileName,
                            JobId = j.JobId,
                            ProcessingTimeMilliSeconds = j.DateTimeUpdatedUtc.GetValueOrDefault().Subtract(j.DateTimeSubmittedUtc).TotalMilliseconds
                        })
                        .OrderBy(x => x.Ukprn)
                        .ThenByDescending(x => x.JobId)
                        .AsQueryable();

                    result = new PaginatedResult<JobDetails>(queryable, pageSize, pageNumber);

                    // get all the names for providers
                    var ukprns = result.List.Select(x => x.Ukprn).Distinct().ToList();
                    var providers = await GetProviders(ukprns);
                    OrgDetail org;

                    // update model with provider name
                    result.List.ForEach(x => x.ProviderName = providers.TryGetValue(x.Ukprn, out org) ? org.Name : null);
                }

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    $"Error occured trying to get job details for status : {jobStatus}, date time start: {startDateTimeUtc} , end date time : {endDateTimeUtc}",
                    exception);
                throw;
            }
        }

        private async Task<Dictionary<long, OrgDetail>> GetProviders(ICollection<long> ukprns)
        {
            using (IOrganisationsContext orgContext = _orgContextFactory())
            {
                return await orgContext.OrgDetails.Where(x => ukprns.Contains(x.Ukprn)).ToDictionaryAsync(s => s.Ukprn);
            }
        }

        private async Task<IEnumerable<T>> GetJobs<T>(string sp, object prm)
            where T : ReadOnlyJobBase
        {
            using (var connection = new SqlConnection(_jobQueueManagerSettings.ConnectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<T>(sp, commandType: CommandType.StoredProcedure, param: prm);
            }
        }
    }
}
