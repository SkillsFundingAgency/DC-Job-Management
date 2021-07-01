using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.JobQueueManager.Interfaces
{
    public interface IValidationRulesService
    {
        int AcademicYear { get; }

        Task<IEnumerable<ValidationRuleDto>> GetILRValidationRulesAsync(CancellationToken cancellationToken);
    }
}
