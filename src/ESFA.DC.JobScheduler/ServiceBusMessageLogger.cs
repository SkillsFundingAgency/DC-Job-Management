using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobContext;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.JobScheduler.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.JobScheduler
{
    public class ServiceBusMessageLogger : IServiceBusMessageLogger
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;
        private readonly JobContextMapper _jobContextMapper;

        public ServiceBusMessageLogger(
            Func<IJobQueueDataContext> contextFactory,
            IJsonSerializationService jsonSerializationService,
            IDateTimeProvider dateTimeProvider,
            ILogger logger,
            JobContextMapper jobContextMapper)
        {
            _contextFactory = contextFactory;
            _jsonSerializationService = jsonSerializationService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _jobContextMapper = jobContextMapper;
        }

        public async Task LogMessageAsync(IJobContextMessage message, CancellationToken cancellationToken)
        {
            try
            {
                using (var context = _contextFactory())
                {
                    context.ServiceBusMessageLog.Add(new ServiceBusMessageLog()
                    {
                        DateTimeCreatedUtc = _dateTimeProvider.GetNowUtc(),
                        JobId = message.JobId,
                        Message = _jsonSerializationService.Serialize(_jobContextMapper.MapFrom((JobContextMessage)message)),
                    });

                    await context.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured writing service bus message log to the db for job id : {message.JobId}", e);
            }
        }
    }
}
