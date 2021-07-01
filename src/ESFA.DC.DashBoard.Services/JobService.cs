using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.DashBoard.Interface;
using ESFA.DC.DashBoard.Models.Job;
using ESFA.DC.DashBoard.Models.Settings;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.DashBoard.Services
{
    public sealed class JobService : IJobService
    {
        private readonly IDASPaymentsService _dasPaymentService;
        private readonly DashBoardJobSettings _jobSettings;
        private readonly ILogger _logger;

        public JobService(
            IDASPaymentsService dasPaymentService,
            DashBoardJobSettings jobSettings,
            ILogger logger)
        {
            _dasPaymentService = dasPaymentService;
            _jobSettings = jobSettings;
            _logger = logger;
        }

        public async Task<JobStatsModel> ProvideAsync(CancellationToken cancellationToken)
        {
            var jobStatsModel = new JobStatsModel
            {
                TodayStatsForYearModel = new List<TodayStatsForYearModel>(),
                SlowFilesComparedToThreePreviousModel = new SlowFilesComparedToThreePreviousModel(),
                JobsCurrentPeriodModels = new List<JobsCurrentPeriodModel>(),
                CurrentPeriodIlrModels = new List<CurrentPeriodIlrModel>(),
                CurrentPeriodSuccessfulUkprnsModels = new List<CurrentPeriodSuccessfulIlrModel>(),
                DasPaymentDifferencesModels = new List<DasPaymentDifferencesModel>(),
                ConcernsModel = new ConcernsModel(),
                CollectionYears = new List<int>()
            };

            try
            {
                using (var connection = new SqlConnection(_jobSettings.ConnectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    SqlMapper.GridReader results = await connection.QueryMultipleAsync(
                        "[dbo].[Dashboard]",
                        commandType: CommandType.StoredProcedure);

                    jobStatsModel.TodayStatsForYearModel = (await results.ReadAsync<TodayStatsForYearModel>()).ToList();
                    jobStatsModel.TodayProcessingTimeModel = (await results.ReadAsync<TodayProcessingTimeModel>()).Single();
                    jobStatsModel.SlowFilesComparedToThreePreviousModel =
                        (await results.ReadAsync<SlowFilesComparedToThreePreviousModel>()).Single();
                    jobStatsModel.JobsCurrentPeriodModels = await results.ReadAsync<JobsCurrentPeriodModel>();
                    jobStatsModel.CurrentPeriodIlrModels = (await results.ReadAsync<CurrentPeriodIlrModel>()).ToList();
                    jobStatsModel.CurrentPeriodSuccessfulUkprnsModels = await results.ReadAsync<CurrentPeriodSuccessfulIlrModel>();
                    jobStatsModel.ConcernsModel = (await results.ReadAsync<ConcernsModel>()).Single();
                    jobStatsModel.CollectionYears = (await results.ReadAsync<int>()).ToList();
                }

                if (jobStatsModel.CurrentPeriodIlrModels.Any())
                {
                    jobStatsModel.DasPaymentDifferencesModels = await GetDasPaymentDifferencesModelsAsync(jobStatsModel, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get dashboard data", ex);
                throw;
            }

            return jobStatsModel;
        }

        private async Task<List<DasPaymentDifferencesModel>> GetDasPaymentDifferencesModelsAsync(JobStatsModel jobStatsModel, CancellationToken cancellationToken)
        {
            var dasPaymentDifferencesModels = new List<DasPaymentDifferencesModel>();
            var dasRequests = new Dictionary<(int Year, int Period), Task<IEnumerable<DasPaymentsModel>>>();

            var yearAndPeriodsReported = jobStatsModel.CurrentPeriodIlrModels.Select(p => new { p.CollectionYear, p.PeriodNumber }).Distinct();

            foreach (var yearAndPeriod in yearAndPeriodsReported)
            {
                dasRequests.Add((Year: yearAndPeriod.CollectionYear, Period: yearAndPeriod.PeriodNumber), _dasPaymentService.GetMissingDASPayments(
                    jobStatsModel.CurrentPeriodSuccessfulUkprnsModels, yearAndPeriod.CollectionYear, yearAndPeriod.PeriodNumber, cancellationToken));
            }

            await Task.WhenAll(dasRequests.Values.ToArray());

            foreach (var response in dasRequests)
            {
                var ukrPrns = response.Value.Result.Select(s => s.Ukprn);

                dasPaymentDifferencesModels.Add(new DasPaymentDifferencesModel
                {
                    UKPrns = ukrPrns,
                    CollectionYear = response.Key.Year,
                    PeriodNumber = response.Key.Period
                });
            }

            return dasPaymentDifferencesModels;
        }
    }
}