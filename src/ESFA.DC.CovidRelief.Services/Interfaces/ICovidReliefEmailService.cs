using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.CovidRelief.Models;

namespace ESFA.DC.CovidRelief.Services.Interfaces
{
    public interface ICovidReliefEmailService
    {
        Task SendEmail(string email, int collectionId);

        Task<int> SendSubmissionNotifications(int collectionId, IEnumerable<string> emails, IEnumerable<SubmissionDate> submissionsByDate);
    }
}
