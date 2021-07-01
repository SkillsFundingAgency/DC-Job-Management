using System.Threading.Tasks;

namespace ESFA.DC.PeriodEnd.Interfaces.Email
{
    public interface IPeriodEndEmailService
    {
        Task PeriodEndEmail(int hubEmailId, int period, string periodPrefix);
    }
}