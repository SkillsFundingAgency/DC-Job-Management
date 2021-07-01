using System;
using System.Collections.Generic;
using Autofac;
using ESFA.DC.CovidRelief.Services;
using ESFA.DC.CovidRelief.Services.Interfaces;
using ESFA.DC.DashBoard.Interface;
using ESFA.DC.DashBoard.Services;
using ESFA.DC.Data.Services;
using ESFA.DC.Data.Services.Interfaces;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR1819.DataStore.EF;
using ESFA.DC.ILR1819.DataStore.EF.Interface;
using ESFA.DC.ILR1920.DataStore.EF;
using ESFA.DC.ILR1920.DataStore.EF.Interface;
using ESFA.DC.ILR2021.DataStore.EF;
using ESFA.DC.ILR2021.DataStore.EF.Interface;
using ESFA.DC.ILR2122.DataStore.EF;
using ESFA.DC.ILR2122.DataStore.EF.Interface;
using ESFA.DC.Job.WebApi.Providers;
using ESFA.DC.Job.WebApi.Settings;
using ESFA.DC.JobContext;
using ESFA.DC.JobNotifications;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager;
using ESFA.DC.JobQueueManager.Audit;
using ESFA.DC.JobQueueManager.Converters;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.ExternalData;
using ESFA.DC.JobQueueManager.Helpers;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using ESFA.DC.JobQueueManager.Interfaces.DataAccess;
using ESFA.DC.JobQueueManager.Interfaces.ExternalData;
using ESFA.DC.JobQueueManager.JobsMetaDataManager;
using ESFA.DC.JobQueueManager.Repositories;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobScheduler;
using ESFA.DC.JobScheduler.Interfaces;
using ESFA.DC.PIMS.EF;
using ESFA.DC.PIMS.EF.Interfaces;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.ReferenceData.Organisations.Model;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using ESFA.DC.ReferenceData.ValidationMessages.EF.Model;
using ESFA.DC.ReferenceData.ValidationMessages.EF.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Summarisation.Model;
using ESFA.DC.Summarisation.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Job.WebApi.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileUploadJob>();
            builder.RegisterType<ReportsPublicationJob>();
            builder.RegisterType<ValidationRuleDetailsReportJob>();

            builder.RegisterType<JobManager>().As<IJobManager>().InstancePerLifetimeScope();
            builder.RegisterType<FileUploadJobManager>().As<IFileUploadJobManager>().InstancePerLifetimeScope();
            builder.RegisterType<ReportsPublicationJobManager>().As<IUpdateJobManager<ReportsPublicationJob>>().InstancePerLifetimeScope();

            builder.RegisterType<ValidationRuleDetailsReportJobManager>().As<IUpdateJobManager<ValidationRuleDetailsReportJob>>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationRuleDetailsReportMetaDataService>().As<IValidationRuleDetailsReportMetaDataService>().InstancePerLifetimeScope();

            builder.RegisterType<JsonSerializationService>().As<ISerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();
            builder.RegisterType<EmailNotifier>().As<IEmailNotifier>().InstancePerLifetimeScope();
            builder.RegisterType<JobEmailTemplateManager>().As<IJobEmailTemplateManager>().InstancePerLifetimeScope();
            builder.RegisterType<ReturnCalendarService>().As<IReturnCalendarService>().InstancePerLifetimeScope();
            builder.RegisterType<OrganisationService>().As<IOrganisationService>().InstancePerLifetimeScope();
            builder.RegisterType<CollectionService>().As<ICollectionService>().InstancePerLifetimeScope();
            builder.RegisterType<JobQueryService>().As<IJobQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<JobManagementService>().As<IJobManagementService>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceMessageService>().As<IServiceMessageService>().InstancePerLifetimeScope();
            builder.RegisterType<ReturnPeriodService>().As<IReturnPeriodService>().InstancePerLifetimeScope();
            builder.RegisterType<ApiAvailabilityService>().As<IApiAvailabilityService>().InstancePerLifetimeScope();
            builder.RegisterType<FisReferenceDataJobMetaDataService>().As<IFisReferenceDataJobMetaDataService>().InstancePerLifetimeScope();

            builder.RegisterType<FailedJobNotificationService>().As<IFailedJobNotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<MessagingService>().As<IMessagingService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageFactory>().As<IMessageFactory>().InstancePerLifetimeScope();
            builder.RegisterType<JobTopicTaskService>().As<IJobTopicTaskService>().InstancePerLifetimeScope();
            builder.RegisterType<ESFReturnPeriodHelper>().As<IESFReturnPeriodHelper>().InstancePerLifetimeScope();
            builder.RegisterType<AppsReturnPeriodHelper>().As<IAppsReturnPeriodHelper>().InstancePerLifetimeScope();
            builder.RegisterType<JobContextMapper>().As<JobContextMapper>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceBusMessageLogger>().As<IServiceBusMessageLogger>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationRules1819Service>().As<IValidationRulesService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationRules1920Service>().As<IValidationRulesService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationRules2021Service>().As<IValidationRulesService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationRules2122Service>().As<IValidationRulesService>().InstancePerLifetimeScope();
            builder.RegisterType<ReferenceDataService>().As<IReferenceDataService>().InstancePerLifetimeScope();
            builder.RegisterType<EsfService>().As<IEsfService>().InstancePerLifetimeScope();
            builder.RegisterType<ReminderService>().As<IReminderService>().InstancePerLifetimeScope();

            builder.RegisterType<ScheduleService>().As<IScheduleService>().InstancePerLifetimeScope();
            builder.RegisterType<ScheduleRepository>().As<IScheduleRepository>().InstancePerLifetimeScope();

            builder.RegisterType<FileUploadJobMetaDataManager>().As<IJobMetaDataManager>().InstancePerLifetimeScope();
            builder.RegisterType<EasJobMetaDataManager>().As<IJobMetaDataManager>().InstancePerLifetimeScope();
            builder.RegisterType<EsfJobMetaDataManager>().As<IJobMetaDataManager>().InstancePerLifetimeScope();
            builder.RegisterType<FisJobMetaDataManager>().As<IJobMetaDataManager>().InstancePerLifetimeScope();
            RegisterDashBoard(builder);

            builder.RegisterType<StorageServiceProvider>().As<IStorageServiceProvider>().ExternallyOwned();
            builder.RegisterType<JobConverter>().As<IJobConverter>().InstancePerLifetimeScope();
            builder.RegisterType<McaDetailService>().As<IMcaDetailService>().InstancePerLifetimeScope();

            builder.RegisterType<IlrDataStoreService>().As<IIlrDataStoreService>().InstancePerLifetimeScope();
            builder.RegisterType<Ilr1819GetSubmissionDetailsService>().As<IIlrGetSubmissionDetailsService>();
            builder.RegisterType<Ilr1920GetSubmissionDetailsService>().As<IIlrGetSubmissionDetailsService>();
            builder.RegisterType<Ilr2021GetSubmissionDetailsService>().As<IIlrGetSubmissionDetailsService>();
            builder.RegisterType<Ilr2122GetSubmissionDetailsService>().As<IIlrGetSubmissionDetailsService>();
            builder.RegisterType<SummarisationDataService>().As<ISummarisationDataService>();

            // Db contexts
            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();
            builder.RegisterType<FcsContext>().As<IFcsContext>().ExternallyOwned();
            builder.RegisterType<ILR1819_DataStoreEntities>().As<IIlr1819RulebaseContext>().ExternallyOwned();
            builder.RegisterType<ILR1920_DataStoreEntities>().As<IIlr1920RulebaseContext>().ExternallyOwned();
            builder.RegisterType<ILR2021_DataStoreEntities>().As<IIlr2021RulebaseContext>().ExternallyOwned();
            builder.RegisterType<ILR2122_DataStoreEntities>().As<IIlr2122Context>().ExternallyOwned();
            builder.RegisterType<ValidationMessagesContext>().As<IValidationMessagesContext>().ExternallyOwned();

            builder.RegisterType<OrganisationsContext>().As<IOrganisationsContext>().ExternallyOwned();
            builder.RegisterType<PimsContext>().As<IPimsContext>().ExternallyOwned();

            builder.RegisterType<JobProcessingService>().As<IJobProcessingService>().InstancePerLifetimeScope();
            builder.RegisterType<CovidReliefSubmissionService>().As<ICovidReliefSubmissionService>().InstancePerLifetimeScope();
            builder.RegisterType<CovidReliefEmailService>().As<ICovidReliefEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<CollectionEmailTemplateManager>().As<ICollectionEmailTemplateManager>().InstancePerLifetimeScope();
            builder.RegisterType<ReportsPublicationJobMetaDataService>().As<IReportsPublicationJobMetaDataService>().InstancePerLifetimeScope();
            builder.RegisterType<ReportsArchiveService>().As<IReportsArchiveService>().InstancePerLifetimeScope();

            builder.RegisterType<AuditContextProvider>().As<IAuditContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AuditFactory>().As<IAuditFactory>().InstancePerLifetimeScope();
            builder.Register(context => {
                var connectionStrings = context.Resolve<ConnectionStrings>();
                var jsonSerializationService = context.Resolve<IJsonSerializationService>();
                return new AuditRepository(jsonSerializationService, connectionStrings.OpsAudit);
            }).As<IAuditRepository>().InstancePerLifetimeScope();

            builder.Register(context =>
                {
                    var queueManagerSettings = context.Resolve<JobQueueManagerSettings>();
                    var optionsBuilder = new DbContextOptionsBuilder<JobQueueDataContext>();
                    optionsBuilder.UseSqlServer(
                        queueManagerSettings.ConnectionString,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<JobQueueDataContext>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<FcsContext>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.FCSReferenceData,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<FcsContext>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<OrganisationsContext>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.ORGReferenceData,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<OrganisationsContext>>()
                .SingleInstance();

            builder.Register(c =>
            {
                var options = c.Resolve<DbContextOptions<OrganisationsContext>>();

                return new OrganisationsContext(options);
            }).As<IOrganisationsContext>().ExternallyOwned();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<SummarisationContext>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.SummarisedActualsData,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<SummarisationContext>>()
                .SingleInstance();

            builder.Register(c =>
            {
                var options = c.Resolve<DbContextOptions<SummarisationContext>>();

                return new SummarisationContext(options);
            }).As<ISummarisationContext>().ExternallyOwned();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<ILR1819_DataStoreEntities>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.ILR1819DataStore,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR1819_DataStoreEntities>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<ILR1920_DataStoreEntities>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.ILR1920DataStore,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));
                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR1920_DataStoreEntities>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<ILR2021_DataStoreEntities>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.ILR2021DataStore,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));
                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR2021_DataStoreEntities>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<ILR2122_DataStoreEntities>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.ILR2122DataStore,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));
                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ILR2122_DataStoreEntities>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<PimsContext>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.PIMSData,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<PimsContext>>()
                .SingleInstance();

            builder.Register(c =>
            {
                var settings = c.Resolve<JobQueueManagerSettings>();
                return new ESFA.DC.JobQueueManager.Settings.JobQueueManagerSettings
                {
                    ConnectionString = settings.ConnectionString,
                    DasPaymentsConnectionString = settings.DasPaymentsConnectionString,
                };
            }).As<ESFA.DC.JobQueueManager.Settings.JobQueueManagerSettings>().SingleInstance();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<ValidationMessagesContext>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.ValidationMessages,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<ValidationMessagesContext>>()
                .SingleInstance();
        }

        private void RegisterDashBoard(ContainerBuilder builder)
        {
            builder.RegisterType<DashBoardService>().As<IDashBoardService>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceBusStatsService>().As<IServiceBusStatsService>().InstancePerLifetimeScope();
            builder.RegisterType<JobService>().As<IJobService>().InstancePerLifetimeScope();
        }
    }
}