using System;
using System.Collections.Generic;
using Autofac;
using ESFA.DC.CovidRelief.EmailService.Configuration;
using ESFA.DC.CovidRelief.EmailService.Interfaces;
using ESFA.DC.CovidRelief.Services;
using ESFA.DC.CovidRelief.Services.Interfaces;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobNotifications;
using ESFA.DC.JobNotifications.Interfaces;
using ESFA.DC.JobQueueManager;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PIMS.EF;
using ESFA.DC.PIMS.EF.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.CovidRelief.EmailService.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();
            builder.RegisterType<EmailNotifier>().As<IEmailNotifier>().InstancePerLifetimeScope();
            builder.RegisterType<CollectionEmailTemplateManager>().As<ICollectionEmailTemplateManager>().InstancePerLifetimeScope();
            builder.RegisterType<CollectionService>().As<ICollectionService>().InstancePerLifetimeScope();
            builder.RegisterType<CovidReliefSubmissionsEmailService>().As<ICovidReliefSubmissionsEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<CovidReliefEmailAddressesService>().As<ICovidReliefEmailAddressesService>().InstancePerLifetimeScope();
            builder.RegisterType<CovidReliefSubmissionService>().As<ICovidReliefSubmissionService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ReturnCalendarService>().As<IReturnCalendarService>().InstancePerLifetimeScope();
            builder.RegisterType<OrganisationService>().As<IOrganisationService>().InstancePerLifetimeScope();
            builder.RegisterType<CovidReliefEmailService>().As<ICovidReliefEmailService>().InstancePerLifetimeScope();

            builder.RegisterType<PimsContext>().As<IPimsContext>().ExternallyOwned();

            builder.Register(c =>
            {
                var connectionStrings = c.Resolve<ConnectionStrings>();
                return new ApplicationLoggerSettings
                {
                    ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>()
                    {
                        new MsSqlServerApplicationLoggerOutputSettings()
                        {
                            MinimumLogLevel = LogLevel.Information,
                            ConnectionString = connectionStrings.AppLogs,
                        },
                    },
                    TaskKey = "Covid Submissions Email Service",
                    JobId = "Covid Submission Email Service",
                };
            }).As<IApplicationLoggerSettings>().SingleInstance();

            builder.RegisterType<ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            builder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SeriLogger>().As<ILogger>().InstancePerLifetimeScope();

            builder.Register(context =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<JobQueueDataContext>();
                    var connectionString = context.Resolve<ConnectionStrings>();

                    optionsBuilder.UseSqlServer(
                        connectionString.JobManagement,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<JobQueueDataContext>>()
                .SingleInstance();
        }
    }
}
