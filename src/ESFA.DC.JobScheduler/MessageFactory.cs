using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobScheduler.Interfaces;
using ESFA.DC.JobScheduler.Interfaces.Models;
using ESFA.DC.JobScheduler.KeyBuilders;

namespace ESFA.DC.JobScheduler
{
    public class MessageFactory : IMessageFactory
    {
        private readonly IJobQueryService _jobQueryService;
        private readonly IJobTopicTaskService _jobTopicTaskService;
        private readonly IESFReturnPeriodHelper _esfCollectionMonthHelper;
        private readonly IAppsReturnPeriodHelper _appsReturnPeriodHelper;
        private readonly IMcaDetailService _mcaDetailService;
        private readonly IReturnCalendarService _returnCalendarService;
        private readonly ICollectionService _collectionService;
        private readonly IValidationRuleDetailsReportMetaDataService _validationRuleDetailsReportMetaDataService;
        private readonly IIndex<string, IJobKeyBuilder<IJobContextMessage, long>> _jobKeyBuilders;

        public MessageFactory(
            IJobQueryService jobQueryService,
            IJobTopicTaskService jobTopicTaskService,
            IESFReturnPeriodHelper esfCollectionMonthHelper,
            IAppsReturnPeriodHelper appsReturnPeriodHelper,
            IMcaDetailService mcaDetailService,
            IReturnCalendarService returnCalendarService,
            ICollectionService collectionService,
            IValidationRuleDetailsReportMetaDataService validationRuleDetailsReportMetaDataService,
            IIndex<string, IJobKeyBuilder<IJobContextMessage, long>> jobKeyBuilders)
        {
            _jobQueryService = jobQueryService;
            _jobTopicTaskService = jobTopicTaskService;
            _esfCollectionMonthHelper = esfCollectionMonthHelper;
            _appsReturnPeriodHelper = appsReturnPeriodHelper;
            _mcaDetailService = mcaDetailService;
            _returnCalendarService = returnCalendarService;
            _collectionService = collectionService;
            _validationRuleDetailsReportMetaDataService = validationRuleDetailsReportMetaDataService;
            _jobKeyBuilders = jobKeyBuilders;
        }

        public async Task<MessageParameters> CreateMessageParametersAsync(string collectionName, long jobId)
        {
            List<ITopicItem> topics;
            string topicName;

            var job = await _jobQueryService.GetJobById(jobId);
            topics = (await _jobTopicTaskService.GetTopicItems(job.CollectionId, !job.IsSubmitted)).ToList();
            topicName = await _jobTopicTaskService.GetTopicName(job.CollectionId);

            return await CreateMessageParametersAsync(collectionName, jobId, topics, topicName);
        }

        public async Task<MessageParameters> CreateMessageParametersAsync(string collectionName, long jobId, List<ITopicItem> topics, string topicName)
        {
            JobContextMessage contextMessage;

            var job = await _jobQueryService.GetJobById(jobId);

            contextMessage = new JobContextMessage(
                job.JobId,
                topics,
                job.Ukprn.ToString(),
                job.StorageReference,
                job.FileName,
                job.CreatedBy,
                0,
                job.DateTimeSubmittedUtc);

            await AddExtraKeysAsync(contextMessage, job);

            MessageParameters message = new MessageParameters(job.CollectionName)
            {
                JobContextMessage = contextMessage,
                SubscriptionLabel = topics[0].SubscriptionName,
                TopicName = topicName,
                TopicParameters = new Dictionary<string, object>
                {
                    {
                        "To", topics[0].SubscriptionName
                    },
                },
            };

            return message;
        }

        private async Task AddExtraKeysAsync(IJobContextMessage message, SubmittedJob job)
        {
            var keys = await _jobTopicTaskService.GetMessageKeysAsync(job.CollectionId, !job.IsSubmitted);
            ValidationRuleDetailsMetaDataModel validationRuleDetailsParameters = null;

            foreach (var key in keys)
            {
                switch (key)
                {
                    case JobContextMessageKey.FileSizeInBytes:
                        message.KeyValuePairs.Add(key, job.FileSize);
                        break;
                    case JobContextMessageKey.PauseWhenFinished:
                        message.KeyValuePairs.Add(key, "1");
                        break;
                    case "OriginalFilename":
                        message.KeyValuePairs.Add(key, job.FileName);
                        break;
                    case "CollectionYear":
                        message.KeyValuePairs.Add(key, job.CollectionYear);
                        break;
                    case "CollectionName":
                        message.KeyValuePairs.Add(key, job.CollectionName);
                        break;
                    case "ReturnPeriod":
                        message.KeyValuePairs.Add(key, job.PeriodNumber);
                        break;
                    case "ReturnPeriodName":
                        message.KeyValuePairs.Add(key, $"R{job.PeriodNumber.ToString("D2", NumberFormatInfo.InvariantInfo)}");
                        break;

                    case "AcademicYearStart":
                        message.KeyValuePairs.Add(key, GenerateYearStart(job.CollectionYear));
                        break;
                    case "AcademicYearEnd":
                        message.KeyValuePairs.Add(key, GenerateYearEnd(job.CollectionYear));
                        break;

                    case JobContextMessageKey.InvalidLearnRefNumbers:
                    case JobContextMessageKey.ValidLearnRefNumbers:
                    case JobContextMessageKey.ValidationErrors:
                    case JobContextMessageKey.ValidationErrorLookups:
                    case JobContextMessageKey.FundingAlbOutput:
                    case JobContextMessageKey.FundingFm35Output:
                    case JobContextMessageKey.FundingFm25Output:
                    case JobContextMessageKey.FundingFm36Output:
                    case "FundingFm70Output":
                    case "FundingFm81Output":
                    case "IlrReferenceData":
                        message.KeyValuePairs.Add(key, GenerateKey(job.Ukprn, job.JobId, key, "json"));
                        break;
                    case "FrmReferenceData":
                        message.KeyValuePairs.Add(key, GenerateKey(job.Ukprn, job.JobId, key, "json"));
                        break;
                    case "LearnerReferenceData":
                        message.KeyValuePairs.Add(key, GenerateKey(job.Ukprn, job.JobId, key, "json"));
                        break;
                    case "TouchpointId":
                        message.KeyValuePairs.Add(key, job.TouchpointId);
                        break;
                    case "ExternalJobId":
                        message.KeyValuePairs.Add(key, job.ExternalJobId);
                        break;
                    case "ExternalTimestamp":
                        message.KeyValuePairs.Add(key, job.ExternalTimestamp);
                        break;
                    case "DssContainer":
                        message.KeyValuePairs.Add(key, job.DssContainer);
                        break;
                    case "ReportEndDate":
                        message.KeyValuePairs.Add(key, job.ReportEndDate);
                        break;
                    case "ReportFileName":
                        message.KeyValuePairs.Add(key, job.ReportFileName);
                        break;

                    case "FundingFm36OutputPeriodEnd":
                        var latestJobKey = await GenerateKey(job.Ukprn, JobContextMessageKey.FundingFm36Output, $"ILR{job.CollectionYear}");
                        message.KeyValuePairs.Add(key, latestJobKey);
                        message.KeyValuePairs.Add(JobContextMessageKey.FundingFm36Output, latestJobKey);
                        break;

                    case "CollectionReturnCodeESF":
                        var period = await _esfCollectionMonthHelper.GetESFReturnPeriod(job.CollectionYear, job.PeriodNumber);
                        message.KeyValuePairs.Add(key, $"ESF{period:D2}");
                        break;
                    case "ProcessTypeESF":
                        message.KeyValuePairs.Add(key, "Deliverable");
                        break;
                    case "CollectionTypeESF":
                        message.KeyValuePairs.Add(key, "ESF");
                        break;
                    case "CollectionReturnCodeDC":
                        message.KeyValuePairs.Add(key, $"R{job.PeriodNumber:D2}");
                        break;
                    case "ProcessTypeDC":
                        message.KeyValuePairs.Add(key, "Fundline");
                        break;
                    case "ProcessTypeGC":
                        message.KeyValuePairs.Add(key, "Generic");
                        break;
                    case "CollectionTypeDC":
                        message.KeyValuePairs.Add(key, $"ILR{job.CollectionYear}");
                        break;
                    case "CollectionReturnCodeApp":
                        var appsPeriod = await _appsReturnPeriodHelper.GetReturnPeriod(job.CollectionYear, job.PeriodNumber);
                        message.KeyValuePairs.Add(key, $"APPS{appsPeriod:D2}");
                        break;
                    case "ProcessTypeApp":
                        message.KeyValuePairs.Add(key, "Payments");
                        break;
                    case "CollectionTypeApp":
                        message.KeyValuePairs.Add(key, "APPS");
                        break;

                    case "CollectionReturnCodeNCS":
                        message.KeyValuePairs.Add(key, $"N{job.PeriodNumber:D2}");
                        break;
                    case "ProcessTypeNCS":
                        message.KeyValuePairs.Add(key, "NCS");
                        break;
                    case "CollectionTypeNCS":
                        message.KeyValuePairs.Add(key, $"NCS{job.CollectionYear}");
                        break;

                    case "McaGlaShortCode":
                        message.KeyValuePairs.Add(key, await _mcaDetailService.GetGLACodeAsync(job.Ukprn));
                        break;
                    case "PreviousPeriodFileReference":
                        message.KeyValuePairs.Add(key, GenerateMcaReportKey(job.Ukprn, job.PeriodNumber));
                        break;
                    case "ILRPeriods":
                        message.KeyValuePairs.Add(key, await _returnCalendarService.GetAllPeriodsAsync($"ILR{job.CollectionYear}"));
                        break;
                    case "SqlJobName":
                        var collectionDescription = (await GetCollectionByNameAsync(CancellationToken.None, job.CollectionName)).Description;
                        message.KeyValuePairs.Add(key, collectionDescription);
                        break;
                    case "ReportsPublicationSourceFolderAndContainer":
                        await _jobKeyBuilders[CollectionTypeConstants.Publication].AddKeys(message, job.JobId);
                        break;
                    case "Rule":
                        validationRuleDetailsParameters = validationRuleDetailsParameters ?? await _validationRuleDetailsReportMetaDataService.GetValidationRuleDetailsReportJobParameters(job.JobId);
                        message.KeyValuePairs.Add(key, validationRuleDetailsParameters.Rule);
                        break;
                    case "SelectedCollectionYear":
                        validationRuleDetailsParameters = validationRuleDetailsParameters ?? await _validationRuleDetailsReportMetaDataService.GetValidationRuleDetailsReportJobParameters(job.JobId);
                        message.KeyValuePairs.Add(key, validationRuleDetailsParameters.SelectedCollectionYear);
                        break;
                    case "SelectedILRPeriods":
                        validationRuleDetailsParameters = validationRuleDetailsParameters ?? await _validationRuleDetailsReportMetaDataService.GetValidationRuleDetailsReportJobParameters(job.JobId);
                        message.KeyValuePairs.Add(key, await _returnCalendarService.GetAllPeriodsAsync($"ILR{validationRuleDetailsParameters.SelectedCollectionYear}"));
                        break;
                    case "McaGlaShortCodeList":
                        var glaCodes = (await _mcaDetailService.GetGLACodesAsync()).ToList();
                        message.KeyValuePairs.Add(key, string.Join(",", glaCodes));
                        break;
                    case "CollectionReturnCodeALLF":
                        message.KeyValuePairs.Add(key, $"A{job.PeriodNumber:D2}");
                        break;
                    case "CollectionTypeALLF":
                        message.KeyValuePairs.Add(key, "ALLF");
                        break;
                    case "CollectionReturnCodeGC":
                        message.KeyValuePairs.Add(key, $"A{job.PeriodNumber:D2}");
                        break;
                    case "CollectionTypeGC":
                        message.KeyValuePairs.Add(key, "ALLF");
                        break;
                    case "HasLarsDataChanged":
                        message.KeyValuePairs.Add(key, true);
                        break;
                    case "MustUpdateLarsSearchIndex":
                        message.KeyValuePairs.Add(key, false);
                        break;
                    case "BlockJob":
                        message.KeyValuePairs.Add(key, true);
                        break;
                }
            }
        }

        private string GenerateKey(long ukprn, long jobId, string value, string extension = null)
        {
            string key = $"{ukprn}/{jobId}/{value}";
            if (string.IsNullOrEmpty(extension))
            {
                return key;
            }

            if (!extension.StartsWith("."))
            {
                key += ".";
            }

            key += extension;

            return key;
        }

        private async Task<string> GenerateKey(long ukprn, string value, string collectionName)
        {
            var latestJob = await _jobQueryService.GetLatestJobByUkprnAsync(ukprn, collectionName, CancellationToken.None);

            if (latestJob != null)
            {
                return GenerateKey(ukprn, latestJob.JobId, value, "json");
            }

            return string.Empty;
        }

        private string GenerateMcaReportKey(long ukprn, int period)
        {
            return period == 1 ? string.Empty : $"R{period - 1:D2}/{ukprn}/Reports.zip";
        }

        private DateTime GenerateYearStart(int collectionYear)
        {
            var startYear = int.Parse("20" + collectionYear.ToString().Substring(0, 2));
            return new DateTime(startYear, 8, 1);
        }

        private DateTime GenerateYearEnd(int collectionYear)
        {
            var endYear = int.Parse("20" + collectionYear.ToString().Substring(2));
            return new DateTime(endYear, 7, 31, 23, 59, 59);
        }

        private async Task<Collection> GetCollectionByNameAsync(CancellationToken cancellationToken, string collectionName)
        {
            return await _collectionService.GetCollectionFromNameAsync(cancellationToken, collectionName);
        }
    }
}