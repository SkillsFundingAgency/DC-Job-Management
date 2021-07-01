using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.PeriodEnd.Interfaces.DataAccess
{
    public interface ICommsRepository
    {
        Task<PathItemEmailDetails> GetEmailDetails(int hubEmailId);
    }
}