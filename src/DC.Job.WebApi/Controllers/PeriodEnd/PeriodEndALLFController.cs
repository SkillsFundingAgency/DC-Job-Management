using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.PeriodEnd
{
    [Produces("application/json")]
    [Route("api/period-end-allf")]
    public class PeriodEndALLFController : BasePeriodEndController
    {
        private readonly IPeriodEndServiceALLF _periodEndService;

        public PeriodEndALLFController(
            ILogger logger,
            IPeriodEndServiceALLF periodEndService)
            : base(logger, periodEndService)
        {
            _periodEndService = periodEndService;
        }

        [HttpGet("file-uploads/{collectionYear}/{period}")]
        public async Task<IEnumerable<JobMetaDataDto>> GetFileUploadsForPeriodAsync(int collectionYear, int period, CancellationToken cancellationToken)
        {
            return await _periodEndService.GetFileUploadsForPeriodAsync(collectionYear, period, cancellationToken);
        }
    }
}