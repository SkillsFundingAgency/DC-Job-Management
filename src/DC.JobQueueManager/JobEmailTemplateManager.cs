using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class JobEmailTemplateManager : IJobEmailTemplateManager
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IReturnCalendarService _returnCalendarService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IJobQueryService _jobQueryService;
        private readonly IEmailNotifier _emailNotifier;
        private readonly ILogger _logger;

        public JobEmailTemplateManager(
            Func<IJobQueueDataContext> contextFactory,
            IReturnCalendarService returnCalendarService,
            IDateTimeProvider dateTimeProvider,
            IJobQueryService jobQueryService,
            IEmailNotifier emailNotifier,
            ILogger logger)
        {
            _contextFactory = contextFactory;
            _returnCalendarService = returnCalendarService;
            _dateTimeProvider = dateTimeProvider;
            _jobQueryService = jobQueryService;
            _emailNotifier = emailNotifier;
            _logger = logger;
        }

        public async Task<string> GetTemplate(long jobId, DateTime dateTimeJobSubmittedUtc)
        {
            using (IJobQueueDataContext context = _contextFactory())
            {
                var job = await context.Job.SingleOrDefaultAsync(x => x.JobId == jobId);

                ReturnPeriod period = null;
                if (job != null)
                {
                    period = await GetReturnPeriod(job.CollectionId, dateTimeJobSubmittedUtc);
                }

                var emailTemplate = await
                    context.JobEmailTemplate.SingleOrDefaultAsync(
                        x => x.CollectionId == job.CollectionId && x.JobStatus == job.Status && x.Active.Value);

                if (emailTemplate == null)
                {
                    return string.Empty;
                }

                if (period != null)
                {
                    return emailTemplate.TemplateOpenPeriod;
                }

                return emailTemplate.TemplateClosePeriod ?? emailTemplate.TemplateOpenPeriod;
            }
        }

        public async Task<string> GetTemplate(int collectionId)
        {
            using (IJobQueueDataContext context = _contextFactory())
            {
                var emailTemplate = await
                    context.JobEmailTemplate.SingleOrDefaultAsync(x => x.CollectionId == collectionId && x.Active.Value);

                return emailTemplate.TemplateOpenPeriod ?? string.Empty;
            }
        }

        public async Task<ReturnPeriod> GetReturnPeriod(int collectionId, DateTime dateTimeUtc)
        {
            if (collectionId == 0)
            {
                return null;
            }

            return await _returnCalendarService.GetPeriodAsync(collectionId, _dateTimeProvider.ConvertUtcToUk(dateTimeUtc));
        }

        public async Task SendEmailNotification(long jobId)
        {
            try
            {
                var job = await _jobQueryService.GetJobById(jobId);
                var template = await GetTemplate(jobId, job.DateTimeSubmittedUtc);

                if (!string.IsNullOrEmpty(template))
                {
                    var personalisation = new Dictionary<string, dynamic>();

                    var submittedAt = _dateTimeProvider.ConvertUtcToUk(job.DateTimeSubmittedUtc);

                    personalisation.Add("JobId", job.JobId);
                    personalisation.Add("Name", job.CreatedBy);
                    personalisation.Add(
                        "DateTimeSubmitted",
                        string.Concat(submittedAt.ToString("hh:mm tt"), " on ", submittedAt.ToString("dddd dd MMMM yyyy")));

                    var nextReturnPeriod = await _returnCalendarService.GetNextPeriodAsync(job.CollectionName);
                    personalisation.Add("FileName", job.FileName);
                    personalisation.Add("CollectionName", job.CollectionName);
                    personalisation.Add(
                        "PeriodName",
                        $"R{job.PeriodNumber.ToString("00", NumberFormatInfo.InvariantInfo)}");
                    personalisation.Add("Ukprn", job.Ukprn);
                    if (nextReturnPeriod != null)
                    {
                        personalisation.Add("NextReturnOpenDate", nextReturnPeriod.StartDateTimeUtc.ToString("dd MMMM yyyy"));
                    }

                    await _emailNotifier.SendEmail(job.NotifyEmail, template, personalisation);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Sending email failed for job {jobId}", ex, jobIdOverride: jobId);
            }
        }
    }
}
