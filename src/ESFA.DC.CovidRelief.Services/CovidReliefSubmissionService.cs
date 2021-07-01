using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.CovidRelief.Models;
using ESFA.DC.CovidRelief.Services.Interfaces;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.CovidRelief.Services
{
    public class CovidReliefSubmissionService : ICovidReliefSubmissionService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IReturnCalendarService _returnCalendarService;
        private readonly IOrganisationService _organisationService;
        private readonly ICovidReliefEmailService _covidReliefEmailService;
        private readonly ILogger _logger;

        public CovidReliefSubmissionService(
            Func<IJobQueueDataContext> contextFactory,
            IDateTimeProvider dateTimeProvider,
            IReturnCalendarService returnCalendarService,
            IOrganisationService organisationService,
            ICovidReliefEmailService covidReliefEmailService,
            ILogger logger)
        {
            _contextFactory = contextFactory;
            _dateTimeProvider = dateTimeProvider;
            _returnCalendarService = returnCalendarService;
            _organisationService = organisationService;
            _covidReliefEmailService = covidReliefEmailService;
            _logger = logger;
        }

        public async Task SubmitAsync(Submission submission)
        {
            try
            {
                var orgDetails = await _organisationService.SearchProvidersInPimsAsync(submission.Ukprn.ToString(), 1);

                if (!orgDetails.Any())
                {
                    throw new ArgumentException("provider not found in PIMS");
                }

                var providerAddress = await _organisationService.GetProviderAddressAsync(submission.Ukprn);

                var period =
                    await _returnCalendarService.GetPeriodAsync(submission.CollectionId, submission.ReturnPeriodNumber);

                using (var context = _contextFactory())
                {
                    var covidSubmissionEntity = new Covid19ReliefSubmission()
                    {
                        CollectionId = submission.CollectionId,
                        Ukprn = submission.Ukprn,
                        FileName = submission.FileName,
                        DateTimeSubmittedUtc = _dateTimeProvider.GetNowUtc(),
                        SubmittedBy = submission.SubmittedBy,
                        ReturnPeriodId = period.ReturnPeriodId,
                        ProviderName = orgDetails[0].Name,
                        Address = string.IsNullOrEmpty(providerAddress?.Address)
                            ? string.Empty
                            : providerAddress.Address
                    };

                    foreach (var question in submission.Questions)
                    {
                        covidSubmissionEntity.Covid19ReliefQuestion.Add(new Covid19ReliefQuestion()
                        {
                            Answer = question.Answer,
                            QuestionNumber = question.QuestionNumber
                        });
                    }

                    context.Covid19ReliefSubmission.Add(covidSubmissionEntity);

                    await context.SaveChangesAsync();

                    await _covidReliefEmailService.SendEmail(submission.ConfirmationEmail, submission.CollectionId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured trying to save covid relief submission data for ukprn : {submission.Ukprn}", ex);
                throw;
            }
        }

        public async Task<bool> HasExistingSubmissionAsync(long ukprn, int collectionId, int periodNumber)
        {
            using (var context = _contextFactory())
            {
                return await context.Covid19ReliefSubmission.AnyAsync(x =>
                    x.Collection.CollectionId == collectionId && x.Ukprn == ukprn &&
                    x.ReturnPeriod.PeriodNumber == periodNumber);
            }
        }

        public async Task<IEnumerable<SubmissionDate>> GetSubmissionsByDate(string collectionType, DateTime dateTime)
        {
            using (var context = _contextFactory())
            {
                return await context.Covid19ReliefSubmission.Where(x => x.Collection.CollectionType.Type == collectionType
                                                                        && x.DateTimeSubmittedUtc.Date == dateTime.Date)
                    .OrderByDescending(x => x.DateTimeSubmittedUtc)
                    .Select(submission => new SubmissionDate()
                    {
                        Ukprn = submission.Ukprn,
                        DateTimeSubmittedUtc = submission.DateTimeSubmittedUtc
                    }).ToListAsync();
            }
        }

        public async Task<List<Submission>> GetSubmissionsListAsync(DateTime dateTimeFromUtc)
        {
            try
            {
                var submissionsList = new List<Submission>();
                using (var context = _contextFactory())
                {
                    submissionsList = await context.Covid19ReliefSubmission
                        .Where(x => x.DateTimeSubmittedUtc >= dateTimeFromUtc).Select(x => new Submission()
                        {
                            SubmittedBy = x.SubmittedBy,
                            Ukprn = x.Ukprn,
                            FileName = x.FileName,
                            CollectionId = x.CollectionId,
                            ReturnPeriodNumber = x.ReturnPeriod.PeriodNumber,
                            SubmittedAt = _dateTimeProvider.ConvertUtcToUk(x.DateTimeSubmittedUtc),
                            ProviderName = x.ProviderName.ToString(),
                            ProviderAddress = x.Address.ToString(),
                            SubmissionId = x.Covid19ReliefSubmissionId,
                        }).OrderByDescending(x => x.SubmittedAt).ToListAsync();
                }

                return submissionsList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured trying to get covid relief submissions list", ex);
                throw;
            }
        }

        public async Task<Submission> GetSubmissionDetailsAsync(int submissionId)
        {
            try
            {
                Submission submissionData = null;
                using (var context = _contextFactory())
                {
                    submissionData = await context.Covid19ReliefSubmission
                        .Where(x => x.Covid19ReliefSubmissionId == submissionId)
                        .Select(x => new Submission
                        {
                            SubmittedBy = x.SubmittedBy,
                            Ukprn = x.Ukprn,
                            FileName = x.FileName,
                            CollectionId = x.CollectionId,
                            ReturnPeriodNumber = x.ReturnPeriod.PeriodNumber,
                            SubmittedAt = _dateTimeProvider.ConvertUtcToUk(x.DateTimeSubmittedUtc),
                            ProviderName = x.ProviderName,
                            ProviderAddress = x.Address,
                            SubmissionId = x.Covid19ReliefSubmissionId,
                            CollectionName = x.Collection.Name,
                            Questions = x.Covid19ReliefQuestion.Select(y =>
                                new SubmissionQuestion()
                                {
                                    Answer = y.Answer,
                                    QuestionNumber = y.QuestionNumber
                                }).ToList()
                        }).SingleOrDefaultAsync();

                    return submissionData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured trying to get covid relief submissions list", ex);
                throw;
            }
        }

        public async Task<CovidReliefAEBMonthlyData> GetCovidReliefAEBMonthlyDataAsync(long ukprn)
        {
            try
            {
                CovidReliefAEBMonthlyData data = null;
                using (var context = _contextFactory())
                {
                    data = await context.Covid19ReliefAebmonthlyCap
                        .Where(x => x.Ukprn == ukprn)
                        .Select(x => new CovidReliefAEBMonthlyData()
                        {
                            EarningsAugust2019 = x.Earningsaug19,
                            EarningsOctober2019 = x.Earningsoct19,
                            EarningsJuly2019 = x.Earningsjuly19,
                            EarningsSeptember2019 = x.Earningssept19,

                            PRS2MonthlyCapJuly = x.Monthlycapjuly,
                            PRS2MonthlyCapAugust = x.Monthlycapaug,
                            PRS2MonthlyCapSeptember = x.Monthlycapsept,
                            PRS2MonthlyCapOctober = x.Monthlycapoct,

                            JulyEarnings201920MCV = x.July1920v2021check,
                            AugustEarnings201920MCV = x.Aug1920v2021check,
                            SeptemberEarnings201920MCV = x.Sept1920v2021check,
                            OctoberEarnings201920MCV = x.Oct1920v2021check,

                            _201819Envelope2 = x._1819env2,
                            _201920Envelope1 = x._1920env1,
                            _201920Envelope2 = x._1920env2,
                            _202021Envelope1 = x._2021env1,

                            _18191920FinancialYearTotalMCV = x._1920fytotal,
                            _202021FinancialYearTotalMCV = x._2020fytoal,

                            JulyEarnings202021MCV = x.Julyearning1920mcv,
                            OctoberEarnings202021MCV = x.Octearning1920mcv,
                            SeptemberEarnings202021MCV = x.Septearning1920mcv,
                            AugustEarnings202021MCV = x.Augearning1920mcv
                        }).SingleOrDefaultAsync();

                    return data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured trying to get GetCovidReliefAEBMonthlyDataAsync for ukprn : {ukprn}", ex);
                throw;
            }
        }

        public async Task<CovidReliefNLMonthlyData> GetCovidReliefNLMonthlyDataAsync(long ukprn)
        {
            try
            {
                CovidReliefNLMonthlyData data = null;
                using (var context = _contextFactory())
                {
                    data = await context.Covid19ReliefNlappsMonthlyCap
                        .Where(x => x.Ukprn == ukprn)
                        .Select(x => new CovidReliefNLMonthlyData()
                        {
                            EarningsAugust2019 = x.Earningsaug19,
                            EarningsOctober2019 = x.Earningsoct19,
                            EarningsJuly2019 = x.Earningsjuly19,
                            EarningsSeptember2019 = x.Earningssept19,

                            PRS2MonthlyCapJuly = x.Monthlycapjuly,
                            PRS2MonthlyCapAugust = x.Monthlycapaug,
                            PRS2MonthlyCapSeptember = x.Monthlycapsept,
                            PRS2MonthlyCapOctober = x.Monthlycapoct,

                            JulyEarnings201920MCV = x.July1920v2021check,
                            AugustEarnings201920MCV = x.Aug1920v2021check,
                            SeptemberEarnings201920MCV = x.Sept1920v2021check,
                            OctoberEarnings201920MCV = x.Oct1920v2021check,

                            _201920MCV = x._1920mcv,
                            _202021MCV = x._2021mcv,

                            JulyEarnings202021MCV = x.Julyearning1920mcv,
                            OctoberEarnings202021MCV = x.Octearning1920mcv,
                            SeptemberEarnings202021MCV = x.Septearning1920mcv,
                            AugustEarnings202021MCV = x.Augearning1920mcv,
                        }).SingleOrDefaultAsync();

                    return data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured trying to get GetCovidReliefAEBMonthlyDataAsync for ukprn : {ukprn}", ex);
                throw;
            }
        }
    }
}
