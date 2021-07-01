using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.PeriodEnd
{
    [Produces("application/json")]
    [Route("api/period-end-ncs")]
    public class PeriodEndNCSController : BasePeriodEndController
    {
        public PeriodEndNCSController(
            ILogger logger,
            IPeriodEndServiceNCS periodEndService)
            : base(logger, periodEndService)
        {
        }
    }
}