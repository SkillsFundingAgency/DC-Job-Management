using System;
using System.Threading.Tasks;
using ESFA.DC.JobContext.Interface;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.JobScheduler.KeyBuilders
{
    public class ReportsPublicationJobKeyBuilder : IJobKeyBuilder<IJobContextMessage, long>
    {
        private const string SourceFolderKey = "SourceFolderKey";
        private const string SourceContainer = "SourceContainer";
        private readonly IReportsPublicationJobMetaDataService _reportsPublicationJobMetaDataService;
        private readonly ILogger _logger;

        public ReportsPublicationJobKeyBuilder(IReportsPublicationJobMetaDataService reportsPublicationJobMetaDataService, ILogger logger)
        {
            _reportsPublicationJobMetaDataService = reportsPublicationJobMetaDataService;
            _logger = logger;
        }

        public async Task AddKeys(IJobContextMessage message, long jobId)
        {
            var parameters = await _reportsPublicationJobMetaDataService.GetFrmReportsJobParameters(jobId);

            if (parameters != null)
            {
                message.KeyValuePairs.Add(SourceFolderKey, parameters.SourceFolderKey);
                message.KeyValuePairs.Add(SourceContainer, parameters.SourceContainerName);
            }
            else
            {
                _logger.LogError($"Publication reports job with jobid : {jobId} being picked up by scheduler - no frm reports parameters available, check ReportsPublicationJobMetaData table", jobIdOverride: jobId);
                throw new Exception("invalid frm reports job");
            }
        }
    }
}
