using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.JobStatus.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Telemetry.Interfaces;

namespace ESFA.DC.JobStatus
{
    public class BaseWebServiceCallService
    {
        protected readonly ILogger _logger;

        protected readonly ITelemetry _telemetry;

        private readonly ISerializationService _serializationService;

        private readonly string _endPointUrl;

        private readonly HttpClient client = new HttpClient();

        protected BaseWebServiceCallService(
            IJobStatusWebServiceCallServiceConfig jobStatusWebServiceCallServiceConfig,
            ISerializationService serializationService,
            ILogger logger,
            ITelemetry telemetry)
        {
            _serializationService = serializationService;
            _logger = logger;
            _endPointUrl = jobStatusWebServiceCallServiceConfig.EndPointUrl;
            _endPointUrl = !_endPointUrl.EndsWith("/") ? $"{_endPointUrl}/Job/Status" : $"{_endPointUrl}Job/Status";
            _telemetry = telemetry;
        }

        protected async Task<HttpResponseMessage> SendStatusAsync(long jobId, int status, CancellationToken cancellationToken, int numOfLearners = -1, bool sendFailureToDas = false)
        {
            try
            {
                _telemetry.TrackEvent(
                    "ILR.FailedJob",
                    new Dictionary<string, string>()
                    {
                        { "JobId", jobId.ToString() },
                        { "status", status.ToString() }
                    },
                    new Dictionary<string, double>()
                    {
                        { "ILR.JobStatusNumberOfLearners", numOfLearners },
                        { "ILR.JobStatusSendFailureToDas", sendFailureToDas ? 1 : 0 }
                    });
            }
            catch (Exception e)
            {
                _logger.LogError("Error occurred in sending telemetry data", e);
            }

            return await client.PostAsync(
                _endPointUrl,
                new StringContent(_serializationService.Serialize(new JobStatusDto(jobId, status, numOfLearners, !sendFailureToDas)), Encoding.UTF8, "application/json"),
                cancellationToken);
        }
    }
}
