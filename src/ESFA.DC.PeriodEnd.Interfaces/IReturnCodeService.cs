using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IReturnCodeService
    {
        Task<CollectionPeriod> GetReturnCodeForPeriodAsync(string collectionType, int year, int period);

        Task<CollectionPeriod> GetReturnCodeForPreviousPeriodAsync(string collectionType, int year, int period);
    }
}
