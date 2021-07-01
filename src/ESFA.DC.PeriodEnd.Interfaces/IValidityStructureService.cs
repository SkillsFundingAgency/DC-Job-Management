using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.PeriodEnd.Interfaces
{
    public interface IValidityStructureService
    {
        Task<PeriodEndValidity> GetAllPeriodEndItems(YearPeriod period, CancellationToken cancellationToken);
    }
}