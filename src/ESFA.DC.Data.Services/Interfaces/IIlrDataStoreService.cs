using System.Threading.Tasks;
using ESFA.DC.Data.Models;

namespace ESFA.DC.Data.Services.Interfaces
{
    public interface IIlrDataStoreService
    {
        Task<IlrFileDetails> GetLatestIlrSubmissionDetails(int academicYear, long ukprn);
    }
}
