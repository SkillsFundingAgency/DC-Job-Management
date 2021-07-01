using System.IO;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.CovidRelief.EmailService.Interfaces;
using ESFA.DC.CovidRelief.EmailService.Ioc;
using ESFA.DC.Logging.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.CovidRelief.EmailService
{
    public static class Program
    {
#if DEBUG
        private const string ConfigFile = "privatesettings.json";
#else
        private const string ConfigFile = "appsettings.json";
#endif

        public static async Task Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigFile);

            var configuration = configBuilder.Build();
            var containerBuilder = new ContainerBuilder();
            var configurationModule = new Autofac.Configuration.ConfigurationModule(configuration);

            containerBuilder.RegisterModule(configurationModule);
            containerBuilder.SetupConfigurations(configuration);
            containerBuilder.RegisterModule<ServiceRegistrations>();

            var container = containerBuilder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var logger = scope.Resolve<ILogger>();
                logger.LogInfo($"Starting Covid relief email service web job.");

                var covidReliefSubmissionsEmailService = scope.Resolve<ICovidReliefSubmissionsEmailService>();
                await covidReliefSubmissionsEmailService.Execute();
            }
        }
    }
}
