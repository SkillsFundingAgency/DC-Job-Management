using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESFA.DC.PeriodEnd.EF
{
    public partial class PeriodEndContext : DbContext
    {
        public PeriodEndContext()
        {
        }

        public PeriodEndContext(DbContextOptions<PeriodEndContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Collection> Collections { get; set; }
        public virtual DbSet<CollectionType> CollectionTypes { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<EmailRecipientGroup> EmailRecipientGroups { get; set; }
        public virtual DbSet<EmailValidityPeriod> EmailValidityPeriods { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Mcadetail> Mcadetails { get; set; }
        public virtual DbSet<Path> Paths { get; set; }
        public virtual DbSet<PathItem> PathItems { get; set; }
        public virtual DbSet<PathItemJob> PathItemJobs { get; set; }
        public virtual DbSet<PeriodEnd> PeriodEnds { get; set; }
        public virtual DbSet<Recipient> Recipients { get; set; }
        public virtual DbSet<RecipientGroup> RecipientGroups { get; set; }
        public virtual DbSet<RecipientGroupRecipient> RecipientGroupRecipients { get; set; }
        public virtual DbSet<ReturnPeriod> ReturnPeriods { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<ValidityPeriod> ValidityPeriods { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\;Database=JobManagement;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Collection>(entity =>
            {
                entity.ToTable("Collection");

                entity.Property(e => e.CollectionId).ValueGeneratedNever();

                entity.Property(e => e.CollectionYear).HasDefaultValueSql("((1819))");

                entity.Property(e => e.Description)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StorageReference).HasMaxLength(100);

                entity.Property(e => e.SubText)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.CollectionType)
                    .WithMany(p => p.Collections)
                    .HasForeignKey(d => d.CollectionTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Collection_CollectionType");
            });

            modelBuilder.Entity<CollectionType>(entity =>
            {
                entity.ToTable("CollectionType");

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
                    .WithMany(p => p.EmailRecipientGroups)
                    .HasForeignKey(d => d.EmailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmailRecipientGroup_Email");

                entity.HasOne(d => d.RecipientGroup)
                    .WithMany(p => p.EmailRecipientGroups)
                    .HasForeignKey(d => d.RecipientGroupId)
                    .HasConstraintName("FK_EmailRecipientGroup_RecipientGroup");
            });

            modelBuilder.Entity<EmailValidityPeriod>(entity =>
            {
                entity.HasKey(e => new { e.HubEmailId, e.Period });

                entity.ToTable("EmailValidityPeriod", "PeriodEnd");

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job");

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
                    .WithMany(p => p.Jobs)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Job_Collection");
            });

            modelBuilder.Entity<Mcadetail>(entity =>
            {
                entity.ToTable("MCADetail");

                entity.HasIndex(e => e.Ukprn)
                    .HasName("UQ__MCADetai__9242AEE32191A19A")
                    .IsUnique();

                entity.Property(e => e.Glacode)
                    .IsRequired()
                    .HasColumnName("GLACode")
                    .HasMaxLength(50);

                entity.Property(e => e.Sofcode).HasColumnName("SOFCode");
            });

            modelBuilder.Entity<Path>(entity =>
            {
                entity.ToTable("Path", "PeriodEnd");

                RelationalReferenceCollectionBuilderExtensions.HasConstraintName((ReferenceCollectionBuilder) entity.HasOne(d => d.PeriodEnd)
                    .WithMany(p => p.Paths)
                    .HasForeignKey(d => d.PeriodEndId)
                    .OnDelete(DeleteBehavior.ClientSetNull), "FK_Path_PeriodEnd");
            });

            modelBuilder.Entity<PathItem>(entity =>
            {
                entity.ToTable("PathItem", "PeriodEnd");

                entity.Property(e => e.IsPausing)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Path)
                    .WithMany(p => p.PathItems)
                    .HasForeignKey(d => d.PathId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PathItem_Path");
            });

            modelBuilder.Entity<PathItemJob>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.PathItemId });

                entity.ToTable("PathItemJob", "PeriodEnd");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.PathItemJobs)
                    .HasForeignKey(d => d.JobId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PathItemJob_Job");

                entity.HasOne(d => d.PathItem)
                    .WithMany(p => p.PathItemJobs)
                    .HasForeignKey(d => d.PathItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PathItemJob_PathItem");
            });

            modelBuilder.Entity<PeriodEnd>(entity =>
            {
                entity.ToTable("PeriodEnd", "PeriodEnd");

                entity.Property(e => e.McareportsPublished).HasColumnName("MCAReportsPublished");

                entity.Property(e => e.McareportsReady).HasColumnName("MCAReportsReady");

                entity.Property(e => e.PeriodEndFinished).HasColumnType("datetime");

                entity.Property(e => e.PeriodEndStarted).HasColumnType("datetime");

                entity.HasOne(d => d.Period)
                    .WithMany(p => p.PeriodEnds)
                    .HasForeignKey(d => d.PeriodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PeriodEnd_Period");
            });

            modelBuilder.Entity<Recipient>(entity =>
            {
                entity.ToTable("Recipient", "Mailing");

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
                    .WithMany(p => p.RecipientGroupRecipients)
                    .HasForeignKey(d => d.RecipientGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipientGroupRecipient_RecipientGroup");

                entity.HasOne(d => d.Recipient)
                    .WithMany(p => p.RecipientGroupRecipients)
                    .HasForeignKey(d => d.RecipientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RecipientGroupRecipient_Recipient");
            });

            modelBuilder.Entity<ReturnPeriod>(entity =>
            {
                entity.ToTable("ReturnPeriod");

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
                    .WithMany(p => p.ReturnPeriods)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ReturnPeriod_Collection");
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CollectionId).HasDefaultValueSql("((1))");

                entity.Property(e => e.LastExecuteDateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Schedule_Collection");
            });

            modelBuilder.Entity<ValidityPeriod>(entity =>
            {
                entity.HasKey(e => new { e.CollectionId, e.Period });

                entity.ToTable("ValidityPeriod", "PeriodEnd");

                entity.Property(e => e.Enabled)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Collection)
                    .WithMany(p => p.ValidityPeriods)
                    .HasForeignKey(d => d.CollectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ValidityPeriod_Collection");
            });
        }
    }
}
