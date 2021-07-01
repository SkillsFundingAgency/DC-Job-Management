using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Ncs.Dss.Service.Configuration;
using ESFA.DC.Ncs.Dss.Service.Dtos;
using ESFA.DC.Ncs.Dss.Service.Extensions;
using ESFA.DC.Ncs.Dss.Service.Interfaces;
using ESFA.DC.Ncs.Dss.Service.Services;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using Microsoft.Extensions.Configuration;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.Ncs.Dss.Service
{
    public static class Program
    {
        public static void Main() // Main entry point
        {
            CompositionRoot().Resolve<Application>().Run();
        }

        public static void SetupConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c => configuration.GetConfigSection<ConnectionStrings>())
                .As<ConnectionStrings>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<AppSettings>())
                .As<AppSettings>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<NcsQueueConfiguration>())
                .As<NcsQueueConfiguration>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<ServiceEndpoints>())
                .As<ServiceEndpoints>().SingleInstance();
        }

        private static IContainer CompositionRoot()
        {
            var builder = new ContainerBuilder();

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());

            var userSettingsFileName = $"privatesettings.json";
            if (File.Exists($"{Directory.GetCurrentDirectory()}/{userSettingsFileName}"))
            {
                config.AddJsonFile(userSettingsFileName);
            }
            else
            {
                config.AddJsonFile("appsettings.json");
            }

            var configurationBuilder = config.Build();
            var configurationModule = new Autofac.Configuration.ConfigurationModule(configurationBuilder);
            builder.RegisterModule(configurationModule);
            builder.SetupConfigurations(configurationBuilder);

            builder.RegisterType<Application>();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();
            builder.Register(c =>
            {
                var connectionString = c.Resolve<ConnectionStrings>();

                return new ApplicationLoggerSettings
                {
                    ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>()
                    {
                        new MsSqlServerApplicationLoggerOutputSettings()
                        {
                            MinimumLogLevel = LogLevel.Verbose,
                            ConnectionString = connectionString.LogConnectionString
                        }
                    },
                    MinimumLogLevel = LogLevel.Verbose
                };
            }).As<IApplicationLoggerSettings>().SingleInstance();

            builder.RegisterType<ExecutionContext>().As<IExecutionContext>().InstancePerLifetimeScope();
            builder.RegisterType<SerilogLoggerFactory>().As<ISerilogLoggerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<SeriLogger>().As<ILogger>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();
            builder.Register(c =>
            {
                var qConfig = c.Resolve<NcsQueueConfiguration>();
                return new QueueSubscriptionService<MessageCrossLoadFromNCSDto>(
                    qConfig,
                    c.Resolve<IJsonSerializationService>(),
                    c.Resolve<ILogger>());
            })
                .As<IQueueSubscriptionService<MessageCrossLoadFromNCSDto>>();

            builder.RegisterType<DssService>().As<IDssService<MessageCrossLoadFromNCSDto>>();
            builder.RegisterType<ExecutionContext>().As<IExecutionContext>()
                .WithParameter("JobId", "Ncs Dss Service")
                .WithParameter("TaskKey", "Ncs Dss");
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();
            builder.RegisterType<JobService>().As<IJobService>();
            builder.RegisterType<HttpClient>().SingleInstance();

            return builder.Build();
        }
    }
}
