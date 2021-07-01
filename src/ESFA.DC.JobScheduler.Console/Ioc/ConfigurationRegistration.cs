using Autofac;
using ESFA.DC.JobManagement.Common;
using ESFA.DC.JobNotifications;
using ESFA.DC.JobScheduler.Settings;
using ESFA.DC.Queueing.Interface.Configuration;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.JobScheduler.Console.Ioc
{
    public static class ConfigurationRegistration
    {
        public static void SetupConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c => configuration.GetConfigSection<JobQueueManagerSettings>())
                .As<JobQueueManagerSettings>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<ServiceBusTopicConfiguration>()).As<ITopicConfiguration>().SingleInstance();
            builder.Register(c => configuration.GetConfigSection<AuditQueueConfiguration>())
                .As<AuditQueueConfiguration>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<ConnectionStrings>())
                .As<ConnectionStrings>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<NotifierConfig>())
                .As<INotifierConfig>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<CrossLoadingQueueConfiguration>())
                .As<IQueueConfiguration>().SingleInstance();
        }
    }
}