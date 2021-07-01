using System;
using System.Collections.Generic;
using Autofac;
using ESFA.DC.Auditing;
using ESFA.DC.Auditing.Dto;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
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
using ESFA.DC.JobQueueManager.Interfaces.ExternalData;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobScheduler.Interfaces;
using ESFA.DC.JobScheduler.KeyBuilders;
using ESFA.DC.JobScheduler.Settings;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.ReferenceData.Organisations.Model;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobScheduler.Console.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobContextMapper>().InstancePerLifetimeScope();
            builder.RegisterType<MessagingService>().As<IMessagingService>().InstancePerLifetimeScope();
            builder.RegisterType<JobManager>().As<IJobManager>().InstancePerLifetimeScope();
            builder.RegisterType<FileUploadJobManager>().As<IFileUploadJobManager>().InstancePerLifetimeScope();
            builder.RegisterType<NcsJobManager>().As<INcsJobManager>().InstancePerLifetimeScope();
            builder.RegisterType<JobQueueHandler>().As<IJobQueueHandler>().InstancePerLifetimeScope();
            builder.RegisterType<JobSchedulerStatusManager>().As<IJobSchedulerStatusManager>().InstancePerLifetimeScope();

            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            builder.RegisterType<EmailNotifier>().As<IEmailNotifier>().InstancePerLifetimeScope();
            builder.RegisterType<JobEmailTemplateManager>().As<IJobEmailTemplateManager>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<ISerializationService>().InstancePerLifetimeScope();

            builder.RegisterType<MessageFactory>().As<IMessageFactory>().SingleInstance();
            builder.RegisterType<ReturnCalendarService>().As<IReturnCalendarService>().InstancePerLifetimeScope();
            builder.RegisterType<CollectionService>().As<ICollectionService>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalDataScheduleService>().As<IExternalDataScheduleService>().InstancePerLifetimeScope();
            builder.RegisterType<ScheduleService>().As<IScheduleService>().InstancePerLifetimeScope();
            builder.RegisterType<JobSchedulerStatusManager>().As<IJobSchedulerStatusManager>().InstancePerLifetimeScope();
            builder.RegisterType<JobTopicTaskService>().As<IJobTopicTaskService>().InstancePerLifetimeScope();
            builder.RegisterType<JobQueryService>().As<IJobQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<JobConverter>().As<IJobConverter>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceBusMessageLogger>().As<IServiceBusMessageLogger>().InstancePerLifetimeScope();

            builder.RegisterType<ESFReturnPeriodHelper>().As<IESFReturnPeriodHelper>().InstancePerLifetimeScope();
            builder.RegisterType<AppsReturnPeriodHelper>().As<IAppsReturnPeriodHelper>().InstancePerLifetimeScope();
            builder.RegisterType<McaDetailService>().As<IMcaDetailService>().InstancePerLifetimeScope();

            builder.RegisterType<ReportsPublicationJobMetaDataService>().As<IReportsPublicationJobMetaDataService>().InstancePerLifetimeScope();
            builder.RegisterType<ReportsPublicationJobKeyBuilder>().Keyed<IJobKeyBuilder<IJobContextMessage, long>>(CollectionTypeConstants.Publication).InstancePerLifetimeScope();

            builder.RegisterType<NcsDssJobMetaDataService>().As<INcsDssJobMetaDataService>().InstancePerLifetimeScope();

            builder.RegisterType<ValidationRuleDetailsReportMetaDataService>().As<IValidationRuleDetailsReportMetaDataService>().InstancePerLifetimeScope();

            builder.RegisterType<AuditContextProvider>().As<IAuditContextProvider>().InstancePerLifetimeScope();
            builder.RegisterType<AuditFactory>().As<IAuditFactory>().InstancePerLifetimeScope();
            builder.Register(context => {
                return new AuditRepository(null, null);
            }).As<IAuditRepository>().InstancePerLifetimeScope();

            builder.Register(c => new QueuePublishService<AuditingDto>(
                    c.Resolve<AuditQueueConfiguration>(),
                    c.Resolve<IJsonSerializationService>()))
                .As<IQueuePublishService<AuditingDto>>();
            builder.RegisterType<Auditor>().As<IAuditor>().InstancePerLifetimeScope();

            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();
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
                var logger = Logging.LoggerManager.CreateDefaultLogger();
                return logger;
            }).As<ILogger>().InstancePerLifetimeScope();

            builder.Register(context =>
                {
                    var connectionStrings = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<OrganisationsContext>();
                    optionsBuilder.UseSqlServer(
                        connectionStrings.Organisation,
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

            builder.Register(c =>
            {
                var settings = c.Resolve<JobQueueManagerSettings>();
                return new ESFA.DC.JobQueueManager.Settings.JobQueueManagerSettings
                {
                    ConnectionString = settings.ConnectionString,
                    DasPaymentsConnectionString = settings.DasPaymentsConnectionString,
                };
            }).As<ESFA.DC.JobQueueManager.Settings.JobQueueManagerSettings>().SingleInstance();
        }
    }
}
