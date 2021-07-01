using System.Collections.Generic;

namespace ESFA.DC.CovidRelief.EmailService.Interfaces
{
    public interface ICovidReliefEmailAddressesService
    {
        IEnumerable<string> GetCovidReliefEmailAddresses(string collectionType);
    }
}
