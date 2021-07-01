using System.Collections.Generic;
using System.IO;
using System.Threading;
using ESFA.DC.Auditing.Interface;
using ESFA.DC.DeadLetter.Service.Configuration;
using ESFA.DC.JobContext;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using Microsoft.Extensions.Configuration;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.DeadLetter.Service
{
    public static class Program
    {
#if DEBUG
        private const string ConfigFile = "privatesettings.json";
#else
        private const string ConfigFile = "appsettings.json";
#endif

        public static void Main(string[] args)
        {
            // Got config
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigFile);

            var configuration = configBuilder.Build();

            var auditingQueueConfiguration = new DeadLetterQueueConfiguration(configuration["ServiceBusConnectionString"], configuration["AuditingQueueName"], 1);
            var failedJobQueueConfiguration = new DeadLetterQueueConfiguration(configuration["ServiceBusConnectionString"], configuration["jobFailedQueueName"], 1);
            var jobStatusQueueConfiguration = new DeadLetterQueueConfiguration(configuration["ServiceBusConnectionString"], configuration["JobStatusQueueName"], 1);
            var emailConfig = new EmailConfig(configuration["NotifierConfig:ApiKey"], configuration["NotifierConfig:Recipients"]);

            // Get Logging
            IApplicationLoggerSettings applicationLoggerOutputSettings = new ApplicationLoggerSettings
            {
                ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>
                {
                    new MsSqlServerApplicationLoggerOutputSettings
                    {
                        ConnectionString = configuration["ConnectionStrings:AppLogs"],
                        MinimumLogLevel = LogLevel.Information
                    },
                    new ConsoleApplicationLoggerOutputSettings
                    {
                        MinimumLogLevel = LogLevel.Information
                    }
                },
                TaskKey = "Dead Letter Status",
                JobId = "Dead Letter Status Service",
                EnableInternalLogs = true,
                MinimumLogLevel = LogLevel.Information
            };
            IExecutionContext executionContext = new ExecutionContext
            {
                TaskKey = "Dead Letter Status",
                JobId = "Dead Letter Status Service",
            };
            ILogger logger = new SeriLogger(applicationLoggerOutputSettings, executionContext);

            // Get dead letter services
            ISerializationService jsonSerializationService = new JsonSerializationService();
            ISerializationService stringSerializationService = new StringSerializationService();

            var auditingQueueDeadLetterRetryService =
                new DeadLetterRetryService<AuditingDto>(
                    auditingQueueConfiguration,
                    emailConfig,
                    stringSerializationService,
                    jsonSerializationService,
                    logger);

            var failedJobQueueDeadLetterRetryService =
                new DeadLetterRetryService<JobContextDto>(
                    failedJobQueueConfiguration,
                    emailConfig,
                    stringSerializationService,
                    jsonSerializationService,
                    logger);

            var jobStatusQueueDeadLetterRetryService =
                new DeadLetterRetryService<JobStatusDto>(
                    jobStatusQueueConfiguration,
                    emailConfig,
                    stringSerializationService,
                    jsonSerializationService,
                    logger);

            // Run until signaled otherwise
            ManualResetEvent oSignalEvent = new ManualResetEvent(false);
            oSignalEvent.WaitOne();
        }
    }
}
