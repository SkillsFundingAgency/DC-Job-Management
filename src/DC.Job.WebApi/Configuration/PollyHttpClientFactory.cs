using System.Net.Http;
using Flurl.Http.Configuration;

namespace ESFA.DC.Job.WebApi.Configuration
{
    public class PollyHttpClientFactory : DefaultHttpClientFactory
    {
        public override HttpMessageHandler CreateMessageHandler()
        {
            return new PolicyHandler
            {
                InnerHandler = base.CreateMessageHandler()
            };
        }
    }
}