using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.PeriodEnd.Models.Config;

namespace ESFA.DC.PeriodEnd.Service.Clients
{
    public class DASClientService : ClientService, IDASClientService
    {
        private const string DASSubmissionEndPoint = "Submission/Successful";
        private const string AcademicYearParameter = "academicYear";
        private const string CollectionPeriodParameter = "collectionPeriod";
        private const string AuthenticationParameter = "code";

        private readonly DASApiSettings _apiSettings;

        public DASClientService(DASApiSettings apiSettings)
        {
            _apiSettings = apiSettings;
        }

        public async Task<IEnumerable<DASSubmission>> GetDASProcessedProviders(int academicYear, int period)
        {
            var parameters = new Dictionary<string, object>
            {
                { AuthenticationParameter, _apiSettings.Authentication },
                { AcademicYearParameter, academicYear },
                { CollectionPeriodParameter, period }
            };

            var result = await GetAsync<DASJobSubmissions>(_apiSettings.BaseUrl, DASSubmissionEndPoint, parameters);

            return result?.SuccessfulSubmissionJobs;
        }
    }
}