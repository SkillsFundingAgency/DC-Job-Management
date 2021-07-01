using System;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class NcsDssJobMetaDataService : INcsDssJobMetaDataService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;

        public NcsDssJobMetaDataService(Func<IJobQueueDataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<NcsDssJobMetaData> GetNcsDssJobParameters(long jobId)
        {
            NcsDssJobMetaData result = null;
            using (IJobQueueDataContext context = _contextFactory())
            {
                var data = await context.NcsJobMetaData.SingleOrDefaultAsync(x => x.JobId == jobId);
                if (data != null)
                {
                    result = new NcsDssJobMetaData()
                    {
                        ExternalJobId = data.ExternalJobId,
                        ExternalTimestamp = data.ExternalTimestamp,
                        ReportFileName = data.ReportFileName,
                        TouchpointId = data.TouchpointId,
                        DssContainer = data.DssContainer,
                        ReportEndDate = data.ReportEndDate,
                    };
                }
            }

            return result;
        }
    }
}
