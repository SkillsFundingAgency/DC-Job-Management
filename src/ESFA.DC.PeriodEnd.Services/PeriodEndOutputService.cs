using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Interfaces.Email;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.PeriodEnd.Utils;

namespace ESFA.DC.PeriodEnd.Services
{
    public sealed class PeriodEndOutputService : IPeriodEndOutputService
    {
        private static int CompletedStatus = Convert.ToInt32(JobStatusType.Completed);
        private static int FailedStatus = Convert.ToInt32(JobStatusType.Failed);
        private static int FailedRetryStatus = Convert.ToInt32(JobStatusType.FailedRetry);

        private readonly IPeriodEndRepository _periodEndRepository;
        private readonly IPeriodEndEmailService _periodEndEmailService;
        private readonly ILogger _logger;

        public PeriodEndOutputService(IPeriodEndRepository periodEndRepository, IPeriodEndEmailService periodEndEmailService, ILogger logger)
        {
            _periodEndRepository = periodEndRepository;
            _periodEndEmailService = periodEndEmailService;
            _logger = logger;
        }

        public async Task PublishProviderReports(int collectionYear, int periodNumber, string collectionType)
        {
            await _periodEndRepository.PublishProviderReportsAsync(collectionYear, periodNumber, collectionType);
        }

        public async Task PublishFm36Reports(int collectionYear, int periodNumber, string collectionType)
        {
            await _periodEndRepository.PublishFm36ReportsAsync(collectionYear, periodNumber, collectionType);
        }

        public async Task PublishMcaReports(int collectionYear, int periodNumber, string collectionType)
        {
            try
            {
                await _periodEndRepository.PublishMcaReportsAsync(collectionYear, periodNumber, collectionType);

                await _periodEndEmailService.PeriodEndEmail(EmailIds.MCAReportsPublishedEmail, periodNumber, PeriodEndConstants.IlrPeriodPrefix);
            }
            catch (Exception e)
            {
                _logger.LogError($"error occured publishing mcs reports for collection year : {collectionYear}, period : {periodNumber}", e);
                throw;
            }
        }

        public async Task CheckCriticalPath(
            PeriodEndState parentStateModel,
            PathPathItemsModel pathModel,
            int year,
            int period,
            string collectionType)
        {
            if (!parentStateModel.Fm36ReportsReady)
            {
                await CheckFm36ReportsRun(pathModel, year, period, collectionType);
            }

            if (!parentStateModel.McaReportsReady)
            {
                await CheckMcaReportJobs(pathModel, year, period, collectionType);
            }

            if (parentStateModel.EsfSummarisationFinished == null)
            {
                await CheckEsfSummarisation(pathModel, year, period, collectionType);
            }

            if (parentStateModel.AppsSummarisationFinished == null)
            {
                await CheckAppsSummarisation(pathModel, year, period, collectionType);
            }

            if (parentStateModel.DcSummarisationFinished == null)
            {
                await CheckDcSummarisation(pathModel, year, period, collectionType);
            }
        }

        public async Task CheckProviderReportJobs(List<PathPathItemsModel> pathModels, int collectionYear, int periodNumber, string collectionType)
        {
            if (!pathModels.Any())
            {
                return;
            }

            if (HaveAllProviderReportsRun(pathModels)
                && HaveAllRunProviderReportsFinished(pathModels))
            {
                await _periodEndRepository.ProviderReportsReadyAsync(collectionYear, periodNumber, collectionType);
            }
        }

        private async Task CheckFm36ReportsRun(PathPathItemsModel pathModel, int collectionYear, int periodNumber, string collectionType)
        {
            var llvPathItem = pathModel.PathItems.SingleOrDefault(pi => pi.PathItemId == PeriodEndPathItem.LLVReport);
            if (llvPathItem?.PathItemJobs != null
                && llvPathItem.PathItemJobs.Any()
                && llvPathItem.PathItemJobs.All(job => job.Status == CompletedStatus))
            {
                await _periodEndRepository.Fm36ReportsReadyAsync(collectionYear, periodNumber, collectionType);
            }
        }

        private bool HaveAllProviderReportsRun(List<PathPathItemsModel> pathModels)
        {
            return !pathModels.Any(p => p.PathItems.Any(pi => pi.IsProviderReport && pi.PathItemJobs?.Any() != true));
        }

        private bool HaveAllRunProviderReportsFinished(List<PathPathItemsModel> pathModels)
        {
            int[] finishedStatuses = { CompletedStatus, FailedStatus, FailedRetryStatus };

            return !pathModels.Any(p => p.PathItems.Any(pi => pi.IsProviderReport
                        && pi.PathItemJobs.Any(job => !finishedStatuses.Contains(job.Status))));
        }

        private async Task CheckEsfSummarisation(PathPathItemsModel pathModel, int year, int period, string collectionType)
        {
            var mcaPathItem = pathModel.PathItems.SingleOrDefault(pi => pi.PathItemId == PeriodEndPathItem.ESFSummarisation);
            if (mcaPathItem?.PathItemJobs != null
                && mcaPathItem.PathItemJobs.Any()
                && mcaPathItem.PathItemJobs.All(job => job.Status == CompletedStatus))
            {
                await _periodEndRepository.EsfSummarisationReadyAsync(year, period, collectionType);
            }
        }

        private async Task CheckAppsSummarisation(PathPathItemsModel pathModel, int year, int period, string collectionType)
        {
            var mcaPathItem = pathModel.PathItems.SingleOrDefault(pi => pi.PathItemId == PeriodEndPathItem.AppSummarisation);
            if (mcaPathItem?.PathItemJobs != null
                && mcaPathItem.PathItemJobs.Any()
                && mcaPathItem.PathItemJobs.All(job => job.Status == CompletedStatus))
            {
                await _periodEndRepository.AppsSummarisationReadyAsync(year, period, collectionType);
            }
        }

        private async Task CheckDcSummarisation(PathPathItemsModel pathModel, int year, int period, string collectionType)
        {
            var mcaPathItem = pathModel.PathItems.SingleOrDefault(pi => pi.PathItemId == PeriodEndPathItem.DCSummarisation);
            if (mcaPathItem?.PathItemJobs != null
                && mcaPathItem.PathItemJobs.Any()
                && mcaPathItem.PathItemJobs.All(job => job.Status == CompletedStatus))
            {
                await _periodEndRepository.DcSummarisationReadyAsync(year, period, collectionType);
            }
        }

        private async Task CheckMcaReportJobs(PathPathItemsModel pathModel, int collectionYear, int periodNumber, string collectionType)
        {
            var mcaPathItem = pathModel.PathItems.SingleOrDefault(pi => pi.PathItemId == PeriodEndPathItem.MCAReports);
            if (mcaPathItem?.PathItemJobs != null
                && mcaPathItem.PathItemJobs.Any()
                && mcaPathItem.PathItemJobs.All(job => job.Status == CompletedStatus))
            {
                await _periodEndRepository.McaReportsReadyAsync(collectionYear, periodNumber, collectionType);
            }
        }
    }
}