using System.Linq;
using Autofac;
using ESFA.DC.DashBoard.Models.Settings;
using ESFA.DC.Job.WebApi.Extensions;
using ESFA.DC.Job.WebApi.Settings;
using ESFA.DC.JobNotifications;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Config;
using ESFA.DC.Queueing.Interface.Configuration;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.Job.WebApi.Ioc
{
    public static class ConfigurationRegistration
    {
        public static void SetupConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c => configuration.GetConfigSection<JobQueueManagerSettings>())
                .As<JobQueueManagerSettings>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<ConnectionStrings>())
                .As<ConnectionStrings>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<NotifierConfig>())
                .As<INotifierConfig>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<ServiceBusTopicConfiguration>())
                .As<ITopicConfiguration>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<ServiceBusConfiguration>())
                .As<ServiceBusConfiguration>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<DASApiSettings>())
                .As<DASApiSettings>().SingleInstance();

            builder.Register(c =>
            {
                return new CloudStorageSettings
                {
                    ConnectionStrings = configuration.GetSection("CloudStorageSettings").GetChildren()
                        .ToDictionary(x => x.Key, y => y.Value)
                };
            })
            .As<CloudStorageSettings>().SingleInstance();

            builder.Register(c =>
            {
                var settings = c.Resolve<JobQueueManagerSettings>();
                return new DashBoardJobSettings
                {
                    ConnectionString = settings.ConnectionString,
                    DasPaymentsConnectionString = settings.DasPaymentsConnectionString
                };
            }).As<DashBoardJobSettings>().SingleInstance();

            builder.Register(c =>
            {
                var settings = c.Resolve<JobQueueManagerSettings>();
                return new DASSettings
                {
                    DasPaymentsConnectionString = settings.DasPaymentsConnectionString
                };
            }).As<DASSettings>().SingleInstance();

            builder.Register(c =>
            {
                var settings = c.Resolve<JobQueueManagerSettings>();
                return new PeriodEndJobSettings
                {
                    ConnectionString = settings.ConnectionString
                };
            }).As<PeriodEndJobSettings>().SingleInstance();

            builder.Register(c =>
            {
                var config = c.Resolve<ServiceBusConfiguration>();
                return new ServiceBusSettings
                {
                    ServiceBusManagementConnectionString = config.ManagementDCConnectionString
                };
            })
            .As<ServiceBusSettings>().SingleInstance();
        }
    }
}