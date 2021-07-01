using ESFA.DC.JobQueueManager.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager.Data
{
    public partial class JobQueueDataContext : DbContext
    {
        public JobQueueDataContext()
        {
        }

        public JobQueueDataContext(DbContextOptions<JobQueueDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApiAvailability> ApiAvailability { get; set; }
        public virtual DbSet<Collection> Collection { get; set; }
        public virtual DbSet<CollectionRelatedLink> CollectionRelatedLink { get; set; }
        public virtual DbSet<CollectionType> CollectionType { get; set; }
        public virtual DbSet<Covid19ReliefAebmonthlyCap> Covid19ReliefAebmonthlyCap { get; set; }
        public virtual DbSet<Covid19ReliefNlappsMonthlyCap> Covid19ReliefNlappsMonthlyCap { get; set; }
        public virtual DbSet<Covid19ReliefQuestion> Covid19ReliefQuestion { get; set; }
        public virtual DbSet<Covid19ReliefReviewComment> Covid19ReliefReviewComment { get; set; }
        public virtual DbSet<Covid19ReliefSubmission> Covid19ReliefSubmission { get; set; }
        public virtual DbSet<CovidReliefPayment> CovidReliefPayment { get; set; }
        public virtual DbSet<EasJobMetaData> EasJobMetaData { get; set; }
        public virtual DbSet<Email> Email { get; set; }
        public virtual DbSet<EmailRecipientGroup> EmailRecipientGroup { get; set; }
        public virtual DbSet<EmailValidityPeriod> EmailValidityPeriod { get; set; }
        public virtual DbSet<EsfJobMetaData> EsfJobMetaData { get; set; }
        public virtual DbSet<FileUploadJobMetaData> FileUploadJobMetaData { get; set; }
        public virtual DbSet<FisJobMetaData> FisJobMetaData { get; set; }
        public virtual DbSet<IlrJobMetaData> IlrJobMetaData { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<JobEmailTemplate> JobEmailTemplate { get; set; }
        public virtual DbSet<JobMessageKey> JobMessageKey { get; set; }
        public virtual DbSet<JobStatusType> JobStatusType { get; set; }
        public virtual DbSet<JobSubscriptionTask> JobSubscriptionTask { get; set; }
        public virtual DbSet<JobTopicSubscription> JobTopicSubscription { get; set; }
        public virtual DbSet<Mcadetail> Mcadetail { get; set; }
        public virtual DbSet<Migration> Migration { get; set; }
        public virtual DbSet<NcsJobMetaData> NcsJobMetaData { get; set; }
        public virtual DbSet<Organisation> Organisation { get; set; }
        public virtual DbSet<OrganisationCollection> OrganisationCollection { get; set; }
        public virtual DbSet<Path> Path { get; set; }
        public virtual DbSet<PathItem> PathItem { get; set; }
        public virtual DbSet<PathItemJob> PathItemJob { get; set; }
        public virtual DbSet<PeriodEnd> PeriodEnd { get; set; }
        public virtual DbSet<Recipient> Recipient { get; set; }
        public virtual DbSet<RecipientGroup> RecipientGroup { get; set; }
        public virtual DbSet<RecipientGroupRecipient> RecipientGroupRecipient { get; set; }
        public virtual DbSet<Reminder> Reminder { get; set; }
        public virtual DbSet<ReminderCertificate> ReminderCertificate { get; set; }
        public virtual DbSet<ReportsArchive> ReportsArchive { get; set; }
        public virtual DbSet<ReportsPublicationJobMetaData> ReportsPublicationJobMetaData { get; set; }
        public virtual DbSet<ReturnPeriod> ReturnPeriod { get; set; }
        public virtual DbSet<ReturnPeriodDisplayOverride> ReturnPeriodDisplayOverride { get; set; }
        public virtual DbSet<ReturnPeriodOrganisationOverride> ReturnPeriodOrganisationOverride { get; set; }
        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<ServiceBusMessageLog> ServiceBusMessageLog { get; set; }
        public virtual DbSet<ServiceMessage> ServiceMessage { get; set; }
        public virtual DbSet<ServicePage> ServicePage { get; set; }
        public virtual DbSet<ServicePageMessage> ServicePageMessage { get; set; }
        public virtual DbSet<SubPathValidityPeriod> SubPathValidityPeriod { get; set; }
        public virtual DbSet<ValidationRuleDetailsReportJobMetaData> ValidationRuleDetailsReportJobMetaData { get; set; }
        public virtual DbSet<ValidityPeriod> ValidityPeriod { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=;Database=JobManagement;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<ApiAvailability>(entity =>
            {
                entity.HasKey(e => new { e.ApiName, e.Process });

                entity.Property(e => e.ApiName).HasMaxLength(200);

                entity.Property(e => e.Process).HasMaxLength(200);

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.Property(e => e.CollectionId).ValueGeneratedNever();

                entity.Property(e => e.CollectionYear).HasDefaultValueSql("((1819))");

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false);

                entity.Property(e => e.ResubmitJob)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.StorageReference).HasMaxLength(100);

                entity.Property(e => e.SubText)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.CollectionType)
                    .WithMany(p => p.Collection)
                    .HasForeignKey(d => d.CollectionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Collection_CollectionType");
            });

            modelBuilder.Entity<CollectionRelatedLink>(entity =>
            {
                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.Url).IsRequired();

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.CollectionRelatedLink)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CollectionRelatedLink_Collection");
            });

            modelBuilder.Entity<CollectionType>(entity =>
            {
                entity.Property(e => e.CollectionTypeId).ValueGeneratedNever();

                entity.Property(e => e.ConcurrentExecutionCount).HasDefaultValueSql("((25))");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Covid19ReliefAebmonthlyCap>(entity =>
            {
                entity.HasKey(e => e.Ukprn);

                entity.ToTable("Covid19ReliefAEBMonthlyCap");

                entity.Property(e => e.Ukprn)
                    .HasColumnName("UKPRN")
                    .ValueGeneratedNever();

                entity.Property(e => e.Aug1920v2021check).HasMaxLength(255);

                entity.Property(e => e.Augearning1920mcv)
                    .HasColumnName("augearning1920mcv")
                    .HasMaxLength(255);

                entity.Property(e => e.Augok).HasMaxLength(255);

                entity.Property(e => e.Earningsaug19)
                    .HasColumnName("earningsaug19")
                    .HasMaxLength(255);

                entity.Property(e => e.Earningsjuly19)
                    .HasColumnName("earningsjuly19")
                    .HasMaxLength(255);

                entity.Property(e => e.Earningsoct19)
                    .HasColumnName("earningsoct19")
                    .HasMaxLength(255);

                entity.Property(e => e.Earningssept19)
                    .HasColumnName("earningssept19")
                    .HasMaxLength(255);

                entity.Property(e => e.EligibleAeb)
                    .HasColumnName("EligibleAEB")
                    .HasMaxLength(255);

                entity.Property(e => e.July1920v2021check).HasMaxLength(255);

                entity.Property(e => e.Julyearning1920mcv)
                    .HasColumnName("julyearning1920mcv")
                    .HasMaxLength(255);

                entity.Property(e => e.Julyok).HasMaxLength(255);

                entity.Property(e => e.Monthlycapaug)
                    .HasColumnName("monthlycapaug")
                    .HasMaxLength(255);

                entity.Property(e => e.Monthlycapjuly)
                    .HasColumnName("monthlycapjuly")
                    .HasMaxLength(255);

                entity.Property(e => e.Monthlycapoct)
                    .HasColumnName("monthlycapoct")
                    .HasMaxLength(255);

                entity.Property(e => e.Monthlycapsept)
                    .HasColumnName("monthlycapsept")
                    .HasMaxLength(255);

                entity.Property(e => e.Oct1920v2021check).HasMaxLength(255);

                entity.Property(e => e.Octearning1920mcv)
                    .HasColumnName("octearning1920mcv")
                    .HasMaxLength(255);

                entity.Property(e => e.Octok).HasMaxLength(255);

                entity.Property(e => e.ProviderName).HasMaxLength(255);

                entity.Property(e => e.Sept1920v2021check).HasMaxLength(255);

                entity.Property(e => e.Septearning1920mcv)
                    .HasColumnName("septearning1920mcv")
                    .HasMaxLength(255);

                entity.Property(e => e.Septok).HasMaxLength(255);

                entity.Property(e => e._1819env2)
                    .HasColumnName("1819env2")
                    .HasMaxLength(255);

                entity.Property(e => e._1920env1)
                    .HasColumnName("1920env1")
                    .HasMaxLength(255);

                entity.Property(e => e._1920env2)
                    .HasColumnName("1920env2")
                    .HasMaxLength(255);

                entity.Property(e => e._1920fytotal)
                    .HasColumnName("1920FYtotal")
                    .HasMaxLength(255);

                entity.Property(e => e._2020fytoal)
                    .HasColumnName("2020FYtoal")
                    .HasMaxLength(255);

                entity.Property(e => e._2021env1)
                    .HasColumnName("2021env1")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Covid19ReliefNlappsMonthlyCap>(entity =>
            {
                entity.HasKey(e => e.Ukprn);

                entity.ToTable("Covid19ReliefNLAppsMonthlyCap");

                entity.Property(e => e.Ukprn)
                    .HasColumnName("UKPRN")
                    .ValueGeneratedNever();

                entity.Property(e => e.Aug1920v2021check).HasMaxLength(255);

                entity.Property(e => e.Augearning1920mcv)
                    .HasColumnName("augearning1920mcv")
                    .HasMaxLength(255);

                entity.Property(e => e.Augok).HasMaxLength(255);

                entity.Property(e => e.Earningsaug19)
                    .HasColumnName("earningsaug19")
                    .HasMaxLength(255);

                entity.Property(e => e.Earningsjuly19)
                    .HasColumnName("earningsjuly19")
                    .HasMaxLength(255);

                entity.Property(e => e.Earningsoct19)
                    .HasColumnName("earningsoct19")
                    .HasMaxLength(255);

                entity.Property(e => e.Earningssept19)
                    .HasColumnName("earningssept19")
                    .HasMaxLength(255);

                entity.Property(e => e.EligibleApps).HasMaxLength(255);

                entity.Property(e => e.July1920v2021check).HasMaxLength(255);

                entity.Property(e => e.Julyearning1920mcv)
                    .HasColumnName("julyearning1920mcv")
                    .HasMaxLength(255);

                entity.Property(e => e.Julyok).HasMaxLength(255);

                entity.Property(e => e.Monthlycapaug)
                    .HasColumnName("monthlycapaug")
                    .HasMaxLength(255);

                entity.Property(e => e.Monthlycapjuly)
                    .HasColumnName("monthlycapjuly")
                    .HasMaxLength(255);

                entity.Property(e => e.Monthlycapoct)
                    .HasColumnName("monthlycapoct")
                    .HasMaxLength(255);

                entity.Property(e => e.Monthlycapsept)
                    .HasColumnName("monthlycapsept")
                    .HasMaxLength(255);

                entity.Property(e => e.Oct1920v2021check).HasMaxLength(255);

                entity.Property(e => e.Octearning1920mcv)
                    .HasColumnName("octearning1920mcv")
                    .HasMaxLength(255);

                entity.Property(e => e.Octok).HasMaxLength(255);

                entity.Property(e => e.ProviderName).HasMaxLength(255);

                entity.Property(e => e.Sept1920v2021check).HasMaxLength(255);

                entity.Property(e => e.Septearning1920mcv)
                    .HasColumnName("septearning1920mcv")
                    .HasMaxLength(255);

                entity.Property(e => e.Septok).HasMaxLength(255);

                entity.Property(e => e._1920mcv)
                    .HasColumnName("1920MCV")
                    .HasMaxLength(255);

                entity.Property(e => e._2021mcv)
                    .HasColumnName("2021MCV")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Covid19ReliefQuestion>(entity =>
            {
                entity.Property(e => e.Answer).IsRequired();

                entity.Property(e => e.QuestionNumber)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Covid19ReliefSubmission)
                    .WithMany(p => p.Covid19ReliefQuestion)
                    .HasForeignKey(d => d.Covid19ReliefSubmissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Covid19ReliefSubmissionData_ToCovid19ReliefSubmissionData");
            });

            modelBuilder.Entity<Covid19ReliefReviewComment>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AddedBy).IsRequired();

                entity.Property(e => e.ApprovedDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Comment).IsRequired();

                entity.Property(e => e.DateTimeAddedUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<Covid19ReliefSubmission>(entity =>
            {
                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.DateTimeSubmittedUtc).HasColumnType("datetime");

                entity.Property(e => e.FileName).IsRequired();

                entity.Property(e => e.ProviderName).IsRequired();

                entity.Property(e => e.SubmittedBy).IsRequired();

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.Covid19ReliefSubmission)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Covid19ReliefMetaData_ToCollection");

                entity.HasOne(d => d.ReturnPeriod)
                    .WithMany(p => p.Covid19ReliefSubmission)
                    .HasForeignKey(d => d.ReturnPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Covid19ReliefMetaData_ToReturnPeriod");
            });

            modelBuilder.Entity<CovidReliefPayment>(entity =>
            {
                entity.HasKey(e => e.Ukprn)
                    .HasName("PK__CovidRel__50F26B713F65FD51");

                entity.Property(e => e.Ukprn)
                    .HasColumnName("UKPRN")
                    .ValueGeneratedNever();

                entity.Property(e => e.AebIlrEarningsR09)
                    .HasColumnName("AEB_ILR_Earnings_R09")
                    .HasColumnType("money");

                entity.Property(e => e.AebIlrEarningsR10)
                    .HasColumnName("AEB_ILR_Earnings_R10")
                    .HasColumnType("money");

                entity.Property(e => e.AebIlrEarningsR11)
                    .HasColumnName("AEB_ILR_Earnings_R11")
                    .HasColumnType("money");

                entity.Property(e => e.AebPrsPaymentR09)
                    .HasColumnName("AEB_PRS Payment_R09")
                    .HasColumnType("money");

                entity.Property(e => e.AebPrsPaymentR10)
                    .HasColumnName("AEB_PRS Payment_R10")
                    .HasColumnType("money");

                entity.Property(e => e.AebPrsPaymentR11)
                    .HasColumnName("AEB_PRS Payment_R11")
                    .HasColumnType("money");

                entity.Property(e => e.NlappsIlrEarningsR09)
                    .HasColumnName("NLAPPS_ILR_Earnings_R09")
                    .HasColumnType("money");

                entity.Property(e => e.NlappsIlrEarningsR10)
                    .HasColumnName("NLAPPS_ILR_Earnings_R10")
                    .HasColumnType("money");

                entity.Property(e => e.NlappsIlrEarningsR11)
                    .HasColumnName("NLAPPS_ILR_Earnings_R11")
                    .HasColumnType("money");

                entity.Property(e => e.NlappsPrsPaymentR09)
                    .HasColumnName("NLAPPS_PRS_Payment_R09")
                    .HasColumnType("money");

                entity.Property(e => e.NlappsPrsPaymentR10)
                    .HasColumnName("NLAPPS_PRS_Payment_R10")
                    .HasColumnType("money");

                entity.Property(e => e.NlappsPrsPaymentR11)
                    .HasColumnName("NLAPPS_PRS_Payment_R11")
                    .HasColumnType("money");
            });

            modelBuilder.Entity<EasJobMetaData>(entity =>
            {
                entity.HasIndex(e => e.JobId)
                    .HasName("IDX_EasJobMetaData_JobId");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.EasJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EasJobMetaData_ToJob");
            });

            modelBuilder.Entity<Email>(entity =>
            {
                entity.ToTable("Email", "Mailing");

                entity.Property(e => e.TemplateId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TemplateName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TriggerPointName)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<EmailRecipientGroup>(entity =>
            {
                entity.ToTable("EmailRecipientGroup", "Mailing");

                entity.HasOne(d => d.Email)
                    .WithMany(p => p.EmailRecipientGroup)
                    .HasForeignKey(d => d.EmailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmailRecipientGroup_Email");

                entity.HasOne(d => d.RecipientGroup)
                    .WithMany(p => p.EmailRecipientGroup)
                    .HasForeignKey(d => d.RecipientGroupId)
                    .HasConstraintName("FK_EmailRecipientGroup_RecipientGroup");
            });

            modelBuilder.Entity<EmailValidityPeriod>(entity =>
            {
                entity.HasKey(e => new { e.HubEmailId, e.CollectionYear, e.Period });

                entity.ToTable("EmailValidityPeriod", "PeriodEnd");

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<EsfJobMetaData>(entity =>
            {
                entity.HasIndex(e => e.JobId)
                    .HasName("IDX_EsfJobMetaData_JobId");

                entity.Property(e => e.ContractReferenceNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.PublishedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.EsfJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EsfMetaData_ToTable");
            });

            modelBuilder.Entity<FileUploadJobMetaData>(entity =>
            {
                entity.HasIndex(e => e.JobId)
                    .HasName("IX_FileUploadJobMetaData_Column");

                entity.HasIndex(e => new { e.JobId, e.FileName, e.PeriodNumber })
                    .HasName("IX_IlrMetaData_PeriodNumber");

                entity.Property(e => e.FileName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.FileSize).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PeriodNumber).HasDefaultValueSql("((1))");

                entity.Property(e => e.StorageReference)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.FileUploadJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FileUploadJobMetaData_ToJob");
            });

            modelBuilder.Entity<FisJobMetaData>(entity =>
            {
                entity.HasIndex(e => e.JobId)
                    .HasName("IDX_FisJobMetaData_JobId");

                entity.Property(e => e.GeneratedDate).HasColumnType("datetime");

                entity.Property(e => e.PublishedDate).HasColumnType("datetime");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.FisJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FisJobMetaData_Job");
            });

            modelBuilder.Entity<IlrJobMetaData>(entity =>
            {
                entity.HasIndex(e => new { e.DateTimeSubmittedUtc, e.JobId })
                    .HasName("IX_IlrMetaData_JobId");

                entity.Property(e => e.DateTimeSubmittedUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.IlrJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IlrJobMetaData_IlrJobMetaData");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasIndex(e => new { e.Status, e.Ukprn, e.CollectionId })
                    .HasName("IDX_Job_CollectionId");

                entity.HasIndex(e => new { e.CollectionId, e.DateTimeCreatedUtc, e.DateTimeUpdatedUtc, e.Status, e.Ukprn })
                    .HasName("IDX_Job_Status_UKPRN");

                entity.HasIndex(e => new { e.DateTimeCreatedUtc, e.DateTimeUpdatedUtc, e.Ukprn, e.CollectionId, e.Status })
                    .HasName("IDX_Job_CollectionId_Status");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DateTimeCreatedUtc)
                    .HasColumnName("DateTimeCreatedUTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.DateTimeUpdatedUtc)
                    .HasColumnName("DateTimeUpdatedUTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.NotifyEmail).HasMaxLength(500);

                entity.Property(e => e.RowVersion).IsRowVersion();

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.Job)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_Collection");
            });

            modelBuilder.Entity<JobEmailTemplate>(entity =>
            {
                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CollectionId).HasDefaultValueSql("((1))");

                entity.Property(e => e.TemplateClosePeriod)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TemplateOpenPeriod)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.JobEmailTemplate)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobEmailTemplate_ToCollection");
            });

            modelBuilder.Entity<JobMessageKey>(entity =>
            {
                entity.Property(e => e.MessageKey)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<JobStatusType>(entity =>
            {
                entity.HasKey(e => e.StatusId);

                entity.Property(e => e.StatusId).ValueGeneratedNever();

                entity.Property(e => e.StatusDescription)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.StatusTitle)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<JobSubscriptionTask>(entity =>
            {
                entity.HasKey(e => e.JobTopicTaskId);

                entity.HasIndex(e => e.JobTopicTaskId)
                    .HasName("IX_JobSubscriptionTask")
                    .IsUnique();

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.TaskName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TaskOrder).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.JobTopic)
                    .WithMany(p => p.JobSubscriptionTask)
                    .HasForeignKey(d => d.JobTopicId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobSubscriptionTask_JobTopic");
            });

            modelBuilder.Entity<JobTopicSubscription>(entity =>
            {
                entity.HasKey(e => e.JobTopicId);

                entity.HasIndex(e => e.JobTopicId)
                    .HasName("IX_JobTopicSubscription")
                    .IsUnique();

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.SubscriptionName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TopicName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.TopicOrder).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.JobTopicSubscription)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobTopicSubscription_ToCollection");
            });

            modelBuilder.Entity<Mcadetail>(entity =>
            {
                entity.ToTable("MCADetail");

                entity.HasIndex(e => e.Ukprn)
                    .HasName("UQ__MCADetai__9242AEE34F79311C")
                    .IsUnique();

                entity.Property(e => e.AcademicYearFrom).HasDefaultValueSql("((2122))");

                entity.Property(e => e.EffectiveFrom)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('2021-08-01')");

                entity.Property(e => e.EffectiveTo).HasColumnType("datetime");

                entity.Property(e => e.Glacode)
                    .IsRequired()
                    .HasColumnName("GLACode")
                    .HasMaxLength(50);

                entity.Property(e => e.Sofcode).HasColumnName("SOFCode");
            });

            modelBuilder.Entity<Migration>(entity =>
            {
                entity.Property(e => e.MigrationId).ValueGeneratedNever();

                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.BuildBranchname)
                    .IsRequired()
                    .HasColumnName("BUILD_BRANCHNAME")
                    .HasMaxLength(150);

                entity.Property(e => e.BuildBuildnumber)
                    .IsRequired()
                    .HasColumnName("BUILD_BUILDNUMBER")
                    .HasMaxLength(150);

                entity.Property(e => e.DateTimeCreatedUtc)
                    .HasColumnName("DateTimeCreatedUTC")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ReleaseReleasename)
                    .IsRequired()
                    .HasColumnName("RELEASE_RELEASENAME")
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<NcsJobMetaData>(entity =>
            {
                entity.HasIndex(e => e.JobId)
                    .HasName("IDX_NcsJobMetaData_JobId");

                entity.Property(e => e.DssContainer)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ExternalJobId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ExternalTimestamp).HasColumnType("datetime");

                entity.Property(e => e.ReportEndDate).HasColumnType("datetime");

                entity.Property(e => e.ReportFileName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TouchpointId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.NcsJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NcsJobMetaData_ToJob");
            });

            modelBuilder.Entity<Organisation>(entity =>
            {
                entity.Property(e => e.IsMca).HasColumnName("IsMCA");
            });

            modelBuilder.Entity<OrganisationCollection>(entity =>
            {
                entity.HasKey(e => new { e.OrganisationId, e.CollectionId });

                entity.Property(e => e.EndDateTimeUtc)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('2600-07-31')");

                entity.Property(e => e.StartDateTimeUtc)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('2018-08-01')");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.OrganisationCollection)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganisationCollection_Collection");

                entity.HasOne(d => d.Organisation)
                    .WithMany(p => p.OrganisationCollection)
                    .HasForeignKey(d => d.OrganisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrganisationCollection_Organisation");
            });

            modelBuilder.Entity<Path>(entity =>
            {
                entity.ToTable("Path", "PeriodEnd");

                entity.Property(e => e.PathLabel)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.PeriodEnd)
                    .WithMany(p => p.Path)
                    .HasForeignKey(d => d.PeriodEndId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Path_PeriodEnd");
            });

            modelBuilder.Entity<PathItem>(entity =>
            {
                entity.ToTable("PathItem", "PeriodEnd");

                entity.Property(e => e.HasJobs)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsPausing)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.PathItemLabel)
                    .IsRequired()
                    .IsUnicode(false);

                entity.HasOne(d => d.Path)
                    .WithMany(p => p.PathItem)
                    .HasForeignKey(d => d.PathId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PathItem_Path");
            });

            modelBuilder.Entity<PathItemJob>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.PathItemId });

                entity.ToTable("PathItemJob", "PeriodEnd");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.PathItemJob)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PathItemJob_Job");

                entity.HasOne(d => d.PathItem)
                    .WithMany(p => p.PathItemJob)
                    .HasForeignKey(d => d.PathItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PathItemJob_PathItem");
            });

            modelBuilder.Entity<PeriodEnd>(entity =>
            {
                entity.ToTable("PeriodEnd", "PeriodEnd");

                entity.Property(e => e.AppsSummarisationFinished).HasColumnType("datetime");

                entity.Property(e => e.DcSummarisationFinished).HasColumnType("datetime");

                entity.Property(e => e.EsfSummarisationFinished).HasColumnType("datetime");

                entity.Property(e => e.Fm36reportsPublished).HasColumnName("FM36ReportsPublished");

                entity.Property(e => e.Fm36reportsReady).HasColumnName("FM36ReportsReady");

                entity.Property(e => e.McareportsPublished).HasColumnName("MCAReportsPublished");

                entity.Property(e => e.McareportsReady).HasColumnName("MCAReportsReady");

                entity.Property(e => e.PeriodEndFinished).HasColumnType("datetime");

                entity.Property(e => e.PeriodEndStarted).HasColumnType("datetime");

                entity.HasOne(d => d.Period)
                    .WithMany(p => p.PeriodEnd)
                    .HasForeignKey(d => d.PeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PeriodEnd_Period");
            });

            modelBuilder.Entity<Recipient>(entity =>
            {
                entity.ToTable("Recipient", "Mailing");

                entity.HasIndex(e => e.EmailAddress)
                    .HasName("UK_EmailAddress")
                    .IsUnique();

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RecipientGroup>(entity =>
            {
                entity.ToTable("RecipientGroup", "Mailing");

                entity.Property(e => e.GroupName)
                    .HasMaxLength(150)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RecipientGroupRecipient>(entity =>
            {
                entity.HasKey(e => new { e.RecipientGroupId, e.RecipientId });

                entity.ToTable("RecipientGroupRecipient", "Mailing");

                entity.HasOne(d => d.RecipientGroup)
                    .WithMany(p => p.RecipientGroupRecipient)
                    .HasForeignKey(d => d.RecipientGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipientGroupRecipient_RecipientGroup");

                entity.HasOne(d => d.Recipient)
                    .WithMany(p => p.RecipientGroupRecipient)
                    .HasForeignKey(d => d.RecipientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipientGroupRecipient_Recipient");
            });

            modelBuilder.Entity<Reminder>(entity =>
            {
                entity.ToTable("Reminder", "Reminder");

                entity.Property(e => e.ClosedDate).HasColumnType("date");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DeadlineDate).HasColumnType("date");

                entity.Property(e => e.ReminderDate).HasColumnType("date");

                entity.Property(e => e.UpdatedBy).HasMaxLength(250);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<ReminderCertificate>(entity =>
            {
                entity.ToTable("ReminderCertificate", "Reminder");

                entity.HasIndex(e => e.ReminderId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Thumbprint)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.HasOne(d => d.Reminder)
                    .WithMany(p => p.ReminderCertificate)
                    .HasForeignKey(d => d.ReminderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReminderCertificate_Reminder");
            });

            modelBuilder.Entity<ReportsArchive>(entity =>
            {
                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.InSld).HasColumnName("inSLD");

                entity.Property(e => e.UploadedBy)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UploadedDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.CollectionType)
                    .WithMany(p => p.ReportsArchive)
                    .HasForeignKey(d => d.CollectionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportsArchive_CollectionType");
            });

            modelBuilder.Entity<ReportsPublicationJobMetaData>(entity =>
            {
                entity.Property(e => e.PeriodNumber).HasDefaultValueSql("((1))");

                entity.Property(e => e.SourceContainerName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.SourceFolderKey)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.StorageReference)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.ReportsPublicationJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReportsPublicationJobMetaData_ToJob");
            });

            modelBuilder.Entity<ReturnPeriod>(entity =>
            {
                entity.HasIndex(e => new { e.CollectionId, e.ReturnPeriodId })
                    .HasName("UC_ReturnPeriod_Key")
                    .IsUnique();

                entity.Property(e => e.EndDateTimeUtc)
                    .HasColumnName("EndDateTimeUTC")
                    .HasColumnType("datetime");

                entity.Property(e => e.StartDateTimeUtc)
                    .HasColumnName("StartDateTimeUTC")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.ReturnPeriod)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReturnPeriod_Collection");
            });

            modelBuilder.Entity<ReturnPeriodDisplayOverride>(entity =>
            {
                entity.Property(e => e.EndDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.StartDateTimeUtc).HasColumnType("datetime");

                entity.HasOne(d => d.ReturnPeriod)
                    .WithMany(p => p.ReturnPeriodDisplayOverride)
                    .HasForeignKey(d => d.ReturnPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReturnPeriodOverride_ReturnPeriod");
            });

            modelBuilder.Entity<ReturnPeriodOrganisationOverride>(entity =>
            {
                entity.HasOne(d => d.Orgaisation)
                    .WithMany(p => p.ReturnPeriodOrganisationOverride)
                    .HasForeignKey(d => d.OrgaisationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReturnPeriodOrganisationOverride_Organisation");

                entity.HasOne(d => d.ReturnPeriod)
                    .WithMany(p => p.ReturnPeriodOrganisationOverride)
                    .HasForeignKey(d => d.ReturnPeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReturnPeriodOrganisationOverride_ReturnPeriod");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CollectionId).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastExecuteDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.Schedule)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Schedule_Collection");
            });

            modelBuilder.Entity<ServiceBusMessageLog>(entity =>
            {
                entity.HasIndex(e => e.JobId)
                    .HasName("IDX_ServiceBusMessageLog_JobId");

                entity.Property(e => e.DateTimeCreatedUtc).HasColumnType("datetime");

                entity.Property(e => e.Message).IsRequired();
            });

            modelBuilder.Entity<ServiceMessage>(entity =>
            {
                entity.ToTable("ServiceMessage", "ServiceMessage");

                entity.Property(e => e.EndDateTimeUtc).HasColumnType("datetime");

                entity.Property(e => e.Headline)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.StartDateTimeUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<ServicePage>(entity =>
            {
                entity.ToTable("ServicePage", "ServiceMessage");

                entity.Property(e => e.DisplayName)
                    .IsRequired();

                entity.Property(e => e.ControllerName)
                    .IsRequired();
            });

            modelBuilder.Entity<ServicePageMessage>(entity =>
            {
                entity.HasKey(e => new { e.PageId, e.MessageId });

                entity.ToTable("ServicePageMessage", "ServiceMessage");

                entity.HasOne(d => d.Message)
                    .WithMany(p => p.ServicePageMessage)
                    .HasForeignKey(d => d.MessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServicePageMessage_ServiceMessage");

                entity.HasOne(d => d.Page)
                    .WithMany(p => p.ServicePageMessage)
                    .HasForeignKey(d => d.PageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ServicePageMessage_ServicePage");
            });

            modelBuilder.Entity<SubPathValidityPeriod>(entity =>
            {
                entity.HasKey(e => new { e.HubPathId, e.CollectionYear, e.Period });

                entity.ToTable("SubPathValidityPeriod", "PeriodEnd");

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ValidationRuleDetailsReportJobMetaData>(entity =>
            {
                entity.Property(e => e.Rule)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.ValidationRuleDetailsReportJobMetaData)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ValidationRuleDetailsReportJobMetaData_ToJob");
            });

            modelBuilder.Entity<ValidityPeriod>(entity =>
            {
                entity.HasKey(e => new { e.HubPathItemId, e.CollectionYear, e.Period });

                entity.ToTable("ValidityPeriod", "PeriodEnd");

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });
        }
    }
}
