using System;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.JobQueueManager
{
    public class ValidationRuleDetailsReportMetaDataService : IValidationRuleDetailsReportMetaDataService
    {
        private readonly Func<IJobQueueDataContext> _contextFactory;

        public ValidationRuleDetailsReportMetaDataService(Func<IJobQueueDataContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<ValidationRuleDetailsMetaDataModel> GetValidationRuleDetailsReportJobParameters(long jobId)
        {
            ValidationRuleDetailsMetaDataModel result = null;
            using (IJobQueueDataContext context = _contextFactory())
            {
                var data = await context.ValidationRuleDetailsReportJobMetaData.SingleOrDefaultAsync(x => x.JobId == jobId);
                if (data != null)
                {
                    result = new ValidationRuleDetailsMetaDataModel
                    {
                        JobId = jobId,
                        Rule = data.Rule,
                        SelectedCollectionYear = data.SelectedCollectionYear
                    };
                }
            }

            return result;
        }
    }
}
