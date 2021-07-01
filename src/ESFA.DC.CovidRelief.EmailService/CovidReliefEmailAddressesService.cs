using System.Collections.Generic;
using System.Linq;
using ESFA.DC.CovidRelief.EmailService.Configuration;
using ESFA.DC.CovidRelief.EmailService.Interfaces;

namespace ESFA.DC.CovidRelief.EmailService
{
    public class CovidReliefEmailAddressesService : ICovidReliefEmailAddressesService
    {
        private readonly EmailConfig _emailConfig;

        public CovidReliefEmailAddressesService(EmailConfig emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public IEnumerable<string> GetCovidReliefEmailAddresses(string collectionType)
        {
            var key = $"{collectionType}EmailAddresses";
            return _emailConfig.EmailAddresses[key].Split(';').ToList();
        }
    }
}
