using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IPeriodEndJobFactory
    {
        Task<FileUploadJob> CreateJobAsync(
            string collectionName,
            int collectionYear,
            int periodNumber,
            long ukPrn = 0,
            string storageReference = "",
            string fileName = "");
    }
}