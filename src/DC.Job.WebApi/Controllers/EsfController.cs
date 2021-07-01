using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/esf")]
    public class EsfController : Controller
    {
        private readonly IEsfService _esfService;

        public EsfController(
            IEsfService esfService)
        {
            _esfService = esfService;
        }

        [HttpPost("jobMetaData/update")]
        public async Task UpdateJobMetaDataAsync([FromBody] EsfJobMetaData esfJobMetaData, CancellationToken cancellationToken)
        {
            if (esfJobMetaData == null)
            {
                return;
            }

            await _esfService.UpdateJobMetaDataAsync(esfJobMetaData, cancellationToken);
        }

        [HttpPost("jobMetaData/create")]
        public async Task CreateJobMetaDataAsync([FromBody] EsfJobMetaData esfJobMetaData, CancellationToken cancellationToken)
        {
            if (esfJobMetaData == null)
            {
                return;
            }

            await _esfService.CreateJobMetaDataAsync(esfJobMetaData, cancellationToken);
        }
    }
}
