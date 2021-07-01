using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.PeriodEnd
{
    [Produces("application/json")]
    [Route("api/period-end-history-ilr")]
    public class HistoryILRController : BaseHistoryController
    {
        public HistoryILRController(
            ILogger logger,
            IHistoryILRService historyService)
            : base(historyService, logger)
        {
        }
    }
}