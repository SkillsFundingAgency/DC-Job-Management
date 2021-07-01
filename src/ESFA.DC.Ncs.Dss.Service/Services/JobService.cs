using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Ncs.Dss.Service.Configuration;
using ESFA.DC.Ncs.Dss.Service.Extensions;
using ESFA.DC.Ncs.Dss.Service.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.Ncs.Dss.Service.Services
{
    public class JobService : BaseHttpClientService, IJobService
    {
        private const string NcsCollectionType = "NCS";
        private readonly string _baseUrl;

        public JobService(
            IJsonSerializationService jsonSerializationService,
            ServiceEndpoints endpoints,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = endpoints.JobApiEndPoint;
        }

        public async Task<long> SubmitJob(NcsJob ncsJob, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await SendDataAsync($"{_baseUrl}/api/job/ncs", ncsJob, cancellationToken);
            long.TryParse(response, out var result);

            return result;
        }

        public async Task<Collection> GetNcsCollectionByPeriodDate(DateTime dateInMessage, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _jsonSerializationService.Deserialize<Collection>(
                 await GetDataAsync($"{_baseUrl}/api/collections/byDate/{NcsCollectionType}?date={dateInMessage.ToApiFormat()}", cancellationToken));
        }

        public async Task<IEnumerable<ReturnPeriod>> GetNcsCollectionPeriodsByCollection(int collectionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _jsonSerializationService.Deserialize<IEnumerable<ReturnPeriod>>(
                await GetDataAsync($"{_baseUrl}/api/returnperiod/collectionId/{collectionId}", cancellationToken));
        }
    }
}