using System.Collections.Generic;
using System.IO;
using System.Threading;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interfaces;
using ESFA.DC.JobStatus.Service.Configuration;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface.Configuration;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Telemetry;
using ESFA.DC.Telemetry.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.JobStatus.Service
{
    public static class Program
    {
        private const string ConfigFile = "appsettings.json";

        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigFile);

            IConfiguration configuration = configBuilder.Build();

            IJobStatusWebServiceCallServiceConfig jobStatusWebServiceCallConfig = new JobStatusWebServiceCallServiceConfig(configuration["jobSchedulerApiEndPoint"]);
            IQueueConfiguration queueConfiguration = new JobStatusQueueConfiguration(configuration["queueConnectionString"], configuration["jobStatusQueueName"]);
            ISerializationService serializationService = new JsonSerializationService();
            IApplicationLoggerSettings applicationLoggerOutputSettings = new ApplicationLoggerSettings
            {
                ApplicationLoggerOutputSettingsCollection = new List<IApplicationLoggerOutputSettings>
                {
                    new MsSqlServerApplicationLoggerOutputSettings
                    {
                        ConnectionString = configuration["logConnectionString"],
                        MinimumLogLevel = LogLevel.Information
                    },
                    new ConsoleApplicationLoggerOutputSettings
                    {
                        MinimumLogLevel = LogLevel.Information
                    }
                },
                TaskKey = "Job Status",
                EnableInternalLogs = true,
                JobId = "Job Status Service",
                MinimumLogLevel = LogLevel.Information
            };
            IExecutionContext executionContext = new ExecutionContext
            {
                JobId = "Job Status Service",
                TaskKey = "Job Status"
            };
            ILogger logger = new SeriLogger(applicationLoggerOutputSettings, executionContext);
            var queueSubscriptionService = new QueueSubscriptionService<JobStatusDto>(queueConfiguration, serializationService, logger);
            TelemetryClient telemetryClient = new TelemetryClient()
            {
                InstrumentationKey = configuration["instrumentationKey"]
            };
            ITelemetry telemetry = new ApplicationInsightsTelemetry(telemetryClient, serializationService as IJsonSerializationService);
            IJobStatusWebServiceCallService<JobStatusDto> jobStatusWebServiceCallService = new JobStatusWebServiceCallService<JobStatusDto>(jobStatusWebServiceCallConfig, queueSubscriptionService, serializationService, logger, telemetry);

            jobStatusWebServiceCallService.Subscribe();

            logger.LogInfo("Started!");

            ManualResetEvent oSignalEvent = new ManualResetEvent(false);
            oSignalEvent.WaitOne();
        }
    }
}
