using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Data.ReadOnlyEntities;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager.Data
{
    public interface IJobQueueDataContext : IDisposable
    {
        DbSet<ApiAvailability> ApiAvailability { get; set; }

        DbSet<Collection> Collection { get; set; }

        DbSet<CollectionRelatedLink> CollectionRelatedLink { get; set; }

        DbSet<CollectionType> CollectionType { get; set; }

        DbSet<EasJobMetaData> EasJobMetaData { get; set; }

        DbSet<EsfJobMetaData> EsfJobMetaData { get; set; }

        DbSet<Email> Email { get; set; }

        DbSet<EmailRecipientGroup> EmailRecipientGroup { get; set; }

        DbSet<EmailValidityPeriod> EmailValidityPeriod { get; set; }

        DbSet<FileUploadJobMetaData> FileUploadJobMetaData { get; set; }

        DbSet<IlrJobMetaData> IlrJobMetaData { get; set; }

        DbSet<Job> Job { get; set; }

        DbSet<JobEmailTemplate> JobEmailTemplate { get; set; }

        DbSet<JobStatusType> JobStatusType { get; set; }

        DbSet<JobSubscriptionTask> JobSubscriptionTask { get; set; }

        DbSet<JobTopicSubscription> JobTopicSubscription { get; set; }

        DbSet<NcsJobMetaData> NcsJobMetaData { get; set; }

        DbSet<Organisation> Organisation { get; set; }

        DbSet<OrganisationCollection> OrganisationCollection { get; set; }

        DbSet<ReturnPeriod> ReturnPeriod { get; set; }

        DbSet<ReturnPeriodDisplayOverride> ReturnPeriodDisplayOverride { get; set; }

        DbSet<ReturnPeriodOrganisationOverride> ReturnPeriodOrganisationOverride { get; set; }

        DbSet<Schedule> Schedule { get; set; }

        DbSet<ServiceMessage> ServiceMessage { get; set; }

        DbSet<ServicePage> ServicePage { get; set; }

        DbSet<ServicePageMessage> ServicePageMessage { get; set; }

        DbSet<JobMessageKey> JobMessageKey { get; set; }

        DbQuery<ReadOnlyJob> ReadOnlyJob { get; set; }

        DbQuery<ReadOnlyJobQueued> ReadOnlyJobQueued { get; set; }

        DbSet<Mcadetail> Mcadetail { get; set; }

        DbSet<ServiceBusMessageLog> ServiceBusMessageLog { get; set; }

        DbSet<ReportsPublicationJobMetaData> ReportsPublicationJobMetaData { get; set; }

        DbSet<Path> Path { get; set; }

        DbSet<PathItem> PathItem { get; set; }

        DbSet<PathItemJob> PathItemJob { get; set; }

        DbSet<PeriodEnd> PeriodEnd { get; set; }

        DbSet<Recipient> Recipient { get; set; }

        DbSet<RecipientGroup> RecipientGroup { get; set; }

        DbSet<RecipientGroupRecipient> RecipientGroupRecipient { get; set; }

        DbSet<ValidityPeriod> ValidityPeriod { get; set; }

        DbSet<SubPathValidityPeriod> SubPathValidityPeriod { get; set; }

        DbQuery<ReadOnlyJobProcessing> ReadOnlyJobProcessing { get; set; }

        DbQuery<ReadOnlyJobSubmitted> ReadOnlyJobSubmitted { get; set; }

        DbQuery<ReadOnlyJobFailedToday> ReadOnlyJobFailedToday { get; set; }

        DbQuery<ReadOnlyJobSlowFile> ReadOnlyJobSlowFile { get; set; }

        DbQuery<ReadOnlyJobConcern> ReadOnlyJobConcern { get; set; }

        DbSet<ValidationRuleDetailsReportJobMetaData> ValidationRuleDetailsReportJobMetaData { get; set; }

        DbSet<Covid19ReliefQuestion> Covid19ReliefQuestion { get; set; }

        DbSet<Covid19ReliefReviewComment> Covid19ReliefReviewComment { get; set; }

        DbSet<Covid19ReliefSubmission> Covid19ReliefSubmission { get; set; }

        DbSet<Covid19ReliefAebmonthlyCap> Covid19ReliefAebmonthlyCap { get; set; }

        DbSet<Covid19ReliefNlappsMonthlyCap> Covid19ReliefNlappsMonthlyCap { get; set; }

        DbSet<ReportsArchive> ReportsArchive { get; set; }

        DbSet<FisJobMetaData> FisJobMetaData { get; set; }

        DbSet<Reminder> Reminder { get; set; }

        DbSet<ReminderCertificate> ReminderCertificate { get; set; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry(object entity);

        Task<IList<T>> FromSqlAsync<T>(CommandType commandType, string sql, object parameters);
    }
}
