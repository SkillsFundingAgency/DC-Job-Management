using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Interfaces.Email;

namespace ESFA.DC.PeriodEnd.Services.Email
{
    public class PeriodEndEmailService : IPeriodEndEmailService
    {
        private readonly IEmailService _emailService;

        public PeriodEndEmailService(
            IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task PeriodEndEmail(int hubEmailId, int period, string periodPrefix)
        {
            var parameters = new Dictionary<string, dynamic>
            {
                { "ReturnPeriod", $"{periodPrefix}{period:D2}" }
            };

            await _emailService.SendEmail(hubEmailId, parameters);
        }
    }
}