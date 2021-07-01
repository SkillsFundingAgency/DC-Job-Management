﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FailedJob.Service.Configuration;
using ESFA.DC.JobContext;
using ESFA.DC.JobStatus;
using ESFA.DC.JobStatus.Interfaces;
using ESFA.DC.Logging;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Enums;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Queueing;
using ESFA.DC.Queueing.Interface;
using ESFA.DC.Queueing.Interface.Configuration;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Telemetry;
using ESFA.DC.Telemetry.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.FailedJob.Service
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
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigFile);

            IConfiguration configuration = configBuilder.Build();

            IJobStatusWebServiceCallServiceConfig auditingPersistenceServiceConfig = new JobStatusWebServiceCallServiceConfig(configuration["jobSchedulerApiEndPoint"]);
            IQueueConfiguration queueConfiguration = new JobStatusQueueConfiguration(configuration["queueConnectionString"], configuration["jobFailedQueueName"], 1);
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
                TaskKey = "Job Fail Status",
                EnableInternalLogs = true,
                JobId = "Job Fail Status Service",
                MinimumLogLevel = LogLevel.Information
            };
            IExecutionContext executionContext = new ExecutionContext
            {
                JobId = "Job Fail Status Service",
                TaskKey = "Job Fail Status"
            };
            ILogger logger = new SeriLogger(applicationLoggerOutputSettings, executionContext);
            IDateTimeProvider dateTimeProvider = new DateTimeProvider.DateTimeProvider();
            IQueueSubscriptionService<JobContextDto> queueSubscriptionService = new QueueSubscriptionService<JobContextDto>(queueConfiguration, serializationService, logger);

            try
            {
                TelemetryClient telemetryClient = new TelemetryClient()
                {
                    InstrumentationKey = configuration["instrumentationKey"]
                };
                ITelemetry telemetry = new ApplicationInsightsTelemetry(telemetryClient, serializationService as IJsonSerializationService);
            }
            catch (Exception e)
            {
                logger.LogError("Exception occured in setting up telemetry client", e);
            }

            IJobStatusWebServiceCallService<JobContextDto> failedJobsWebServiceCallService = new FailedJobsWebServiceCallService(auditingPersistenceServiceConfig, queueSubscriptionService, serializationService, logger, null);

            failedJobsWebServiceCallService.Subscribe();

            logger.LogInfo($"Started {executionContext.JobId}!");

            ManualResetEvent oSignalEvent = new ManualResetEvent(false);
            oSignalEvent.WaitOne();
        }
    }
}
