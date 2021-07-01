using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESFA.DC.PeriodEnd.Interfaces.Email
{
    public interface IEmailService
    {
        Task SendEmail(int emailId, Dictionary<string, dynamic> emailParameters);
    }
}