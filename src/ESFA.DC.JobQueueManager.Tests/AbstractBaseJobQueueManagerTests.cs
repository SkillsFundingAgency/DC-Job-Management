using System;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.Audit.Models.DTOs.FRM;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager.Converters;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.JobQueueManager.JobsMetaDataManager;
using ESFA.DC.JobQueueManager.Settings;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.ReferenceData.Organisations.Model;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using Path = System.IO.Path;

namespace ESFA.DC.JobQueueManager.Tests
{
    public abstract class AbstractBaseJobQueueManagerTests
    {
        protected void SetupDatabaseForJobs(JobQueueDataContext context)
        {
            context.Database.EnsureCreated();

            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = string.Format("{0}\\{1}", directory, "ReadOnlyJob.sql");
            var source = System.IO.File.ReadAllText($"{directory}\\LinkedItems\\ReadOnlyJob.sql");
            context.Database.ExecuteSqlCommand(source);
        }

        protected Collection SetupCollections(JobQueueDataContext context, string collectionName = "ILR2021", short collectionTypeId = 1, bool IsMultiStage = true, int collectionYear = 1819)
        {
            context.CollectionType.Add(new CollectionType()
            {
                CollectionTypeId = collectionTypeId,
                Description = "submission",
                Type = "ILR",
            });

            var collection = new Collection()
            {
                CollectionId = collectionTypeId,
                CollectionTypeId = collectionTypeId,
                Name = collectionName,
                MultiStageProcessing = IsMultiStage,
                CollectionYear = collectionYear,
            };

            context.Collection.Add(collection);
            context.SaveChanges();

            return collection;
        }

        protected Collection SetupCollectionsNotMulti(JobQueueDataContext context, string collectionName = "ILR2021", short collectionTypeId = 1, bool isMultiStage = false, int collectionYear = 1819)
        {
            context.CollectionType.Add(new CollectionType()
            {
                CollectionTypeId = collectionTypeId,
                Description = "submission",
                Type = "ILR",
            });

            var collection = new Collection()
            {
                CollectionId = collectionTypeId,
                CollectionTypeId = collectionTypeId,
                Name = collectionName,
                MultiStageProcessing = isMultiStage,
                CollectionYear = collectionYear,
            };

            context.Collection.Add(collection);
            context.SaveChanges();

            return collection;
        }

        protected Collection SetupCollectionsPublication(JobQueueDataContext context, string collectionName = "frm1920-files", short collectionTypeId = 1, bool isMultiStage = true, int collectionYear = 1819)
        {
            context.CollectionType.Add(new CollectionType()
            {
                CollectionTypeId = collectionTypeId,
                Description = "submission",
                Type = "Publication",
            });

            var collection = new Collection()
            {
                CollectionId = collectionTypeId,
                CollectionTypeId = collectionTypeId,
                Name = collectionName,
                MultiStageProcessing = isMultiStage,
                CollectionYear = collectionYear,
            };

            context.Collection.Add(collection);
            context.SaveChanges();

            return collection;
        }

        protected Collection SetupCollectionsValidationRuleReport(JobQueueDataContext context, string collectionName = "OP-Validation-Report", short collectionTypeId = 8, bool isMultiStage = true, int collectionYear = 1920)
        {
            context.CollectionType.Add(new CollectionType()
            {
                CollectionTypeId = collectionTypeId,
                Description = "Operations",
                Type = "OP",
            });

            var collection = new Collection()
            {
                CollectionId = collectionTypeId,
                CollectionTypeId = collectionTypeId,
                Name = collectionName,
                MultiStageProcessing = isMultiStage,
                CollectionYear = collectionYear,
            };

            context.Collection.Add(collection);
            context.SaveChanges();

            return collection;
        }

        protected IContainer GetRegistrations(IJobEmailTemplateManager jobEmailTemplateManager = null, IEmailNotifier emailNotifier = null, IDateTimeProvider dateTimeProvider = null, IReturnCalendarService returnCalendarService = null, IAuditFactory auditFactory = null)
        {
            ContainerBuilder builder = new ContainerBuilder();

            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            dateTimeProviderMock.Setup(x => x.GetNowUtc()).Returns(DateTime.UtcNow);

            var auditFactoryMock = new Mock<IAuditFactory>();
            auditFactoryMock.Setup(x => x.BuildDataAudit(It.IsAny<Func<IJobQueueDataContext, Task<FrmUnpublishDTO>>>(), It.IsAny<IJobQueueDataContext>()))
                .Returns(new Mock<IAudit>().Object);
            auditFactoryMock.Setup(x => x.BuildDataAudit(It.IsAny<Func<IJobQueueDataContext, Task<FRMPublishUpdateFlagDTO>>>(), It.IsAny<IJobQueueDataContext>()))
                .Returns(new Mock<IAudit>().Object);

            builder.RegisterInstance(dateTimeProvider ?? dateTimeProviderMock.Object).As<IDateTimeProvider>().SingleInstance();
            builder.RegisterInstance(emailNotifier ?? new Mock<IEmailNotifier>().Object).As<IEmailNotifier>().SingleInstance();
            builder.RegisterInstance(new Mock<IFileUploadJobManager>().Object).As<IFileUploadJobManager>().SingleInstance();
            builder.RegisterInstance(jobEmailTemplateManager ?? new Mock<IJobEmailTemplateManager>().Object).As<IJobEmailTemplateManager>().SingleInstance();
            builder.RegisterInstance(new Mock<ILogger>().Object).As<ILogger>().SingleInstance();
            builder.RegisterInstance(returnCalendarService ?? new Mock<IReturnCalendarService>().Object).As<IReturnCalendarService>().SingleInstance();
            builder.RegisterInstance(auditFactory ?? auditFactoryMock.Object).As<IAuditFactory>().SingleInstance();

            builder.RegisterType<FileUploadJobManager>().As<IFileUploadJobManager>().InstancePerLifetimeScope();
            builder.RegisterType<JobQueryService>().As<IJobQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<CollectionService>().As<ICollectionService>().SingleInstance();
            builder.RegisterType<JobConverter>().As<IJobConverter>().SingleInstance();
            builder.RegisterType<JobManager>().As<IJobManager>().InstancePerLifetimeScope();
            builder.RegisterType<ReportsPublicationJobManager>().As<IUpdateJobManager<ReportsPublicationJob>>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationRuleDetailsReportJobManager>().As<IUpdateJobManager<ValidationRuleDetailsReportJob>>().InstancePerLifetimeScope();
            builder.RegisterType<FileUploadJobMetaDataManager>().As<IJobMetaDataManager>().InstancePerLifetimeScope();
            builder.RegisterType<EasJobMetaDataManager>().As<IJobMetaDataManager>().InstancePerLifetimeScope();
            builder.RegisterType<EsfJobMetaDataManager>().As<IJobMetaDataManager>().InstancePerLifetimeScope();
            builder.RegisterType<NcsDssJobMetaDataService>().As<INcsDssJobMetaDataService>().InstancePerLifetimeScope();
            builder.RegisterType<NcsJobManager>().As<INcsJobManager>().InstancePerLifetimeScope();
            builder.RegisterType<JobProcessingService>().As<IJobProcessingService>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<ReportsPublicationJobMetaDataService>().As<IReportsPublicationJobMetaDataService>().ExternallyOwned();

            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();
            builder.RegisterType<OrganisationsContext>().As<IOrganisationsContext>().ExternallyOwned();

            builder.Register(c =>
            {
                return new JobQueueManagerSettings
                {
                    ConnectionString = string.Empty
                };
            }).As<JobQueueManagerSettings>().SingleInstance();

            builder.Register(context =>
            {
                SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                DbContextOptionsBuilder<JobQueueDataContext> optionsBuilder =
                    new DbContextOptionsBuilder<JobQueueDataContext>()
                        .UseSqlite(connection);

                return optionsBuilder.Options;
            })
                    .As<DbContextOptions<JobQueueDataContext>>()
                    .SingleInstance();

            builder.Register(context =>
                {
                    SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();
                    DbContextOptionsBuilder<OrganisationsContext> optionsBuilder =
                        new DbContextOptionsBuilder<OrganisationsContext>()
                            .UseSqlite(connection);

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<OrganisationsContext>>()
                .SingleInstance();

            IContainer container = builder.Build();
            return container;
        }
    }
}
