using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace ESFA.DC.PeriodEnd.Service.Clients
{
    public class ClientService
    {
        public async Task<TResult> PostAsync<TContent, TResult>(string baseUrl, string endPoint, TContent content)
        {
            return await baseUrl
                .AppendPathSegment(endPoint)
                .PostJsonAsync(content)
                .ReceiveJson<TResult>();
        }

        public async Task<TResult> GetAsync<TResult>(string baseUrl, string endPoint, IDictionary<string, object> parameters)
        {
            var clientUrl = baseUrl.AppendPathSegment(endPoint);

            foreach (var parameter in parameters)
            {
                clientUrl.SetQueryParam(parameter.Key, parameter.Value);
            }

            return await clientUrl.GetJsonAsync<TResult>();
        }
    }
}