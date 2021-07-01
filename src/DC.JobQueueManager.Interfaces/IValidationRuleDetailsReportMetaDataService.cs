using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IValidationRuleDetailsReportMetaDataService
    {
        Task<ValidationRuleDetailsMetaDataModel> GetValidationRuleDetailsReportJobParameters(long jobId);
    }
}
