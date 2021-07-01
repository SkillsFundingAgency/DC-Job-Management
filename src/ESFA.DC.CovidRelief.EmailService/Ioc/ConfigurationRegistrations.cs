using System;
using System.Linq;
using Autofac;
using ESFA.DC.CovidRelief.EmailService.Configuration;
using ESFA.DC.JobManagement.Common;
using ESFA.DC.JobNotifications;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.CovidRelief.EmailService.Ioc
{
    public static class ConfigurationRegistrations
    {
        public static void SetupConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c => configuration.GetConfigSection<ConnectionStrings>())
                .As<ConnectionStrings>().SingleInstance();

            builder.Register(c =>
                {
                    return new EmailConfig
                    {
                        EmailAddresses = configuration.GetSection("EmailConfig").GetChildren()
                            .ToDictionary(x => x.Key, y => y.Value, StringComparer.InvariantCultureIgnoreCase)
                    };
                })
                .As<EmailConfig>().SingleInstance();

            builder.Register(c => configuration.GetConfigSection<NotifierConfig>())
                .As<INotifierConfig>().SingleInstance();
        }
    }
}
