using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.PeriodEnd
{
    [Produces("application/json")]
    [Route("api/period-end-history-ncs")]
    public class HistoryNCSController : BaseHistoryController
    {
        public HistoryNCSController(
            ILogger logger,
            IHistoryNCSService historyService)
            : base(historyService, logger)
        {
        }
    }
}