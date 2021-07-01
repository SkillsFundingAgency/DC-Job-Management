using System.Threading.Tasks;
using ESFA.DC.Data.Models;

namespace ESFA.DC.Data.Services.Interfaces
{
    public interface IIlrGetSubmissionDetailsService
    {
        int AcademicYear { get; }

        Task<IlrFileDetails> GetIlrSubmissionDetails(long ukprn);
    }
}
