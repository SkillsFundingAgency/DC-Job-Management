using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Job.WebApi.Configuration
{
    public class PolicyHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Policies.PolicyStrategy.ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }
    }
}