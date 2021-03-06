using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.Ncs.Dss.Service.Services
{
    public abstract class BaseHttpClientService
    {
        protected readonly IJsonSerializationService _jsonSerializationService;
        protected readonly HttpClient _httpClient;

        public BaseHttpClientService(
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient)
        {
            _jsonSerializationService = jsonSerializationService;
            _httpClient = httpClient;
        }

        public async Task<string> SendDataAsync(string url, object data, CancellationToken cancellationToken)
        {
            var json = _jsonSerializationService.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetDataAsync(string url, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(new Uri(url), cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
