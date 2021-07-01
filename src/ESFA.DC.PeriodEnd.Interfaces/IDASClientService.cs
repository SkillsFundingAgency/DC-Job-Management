using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IDASClientService
    {
        Task<IEnumerable<DASSubmission>> GetDASProcessedProviders(int academicYear, int period);
    }
}