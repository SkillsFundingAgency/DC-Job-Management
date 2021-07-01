using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.PeriodEnd
{
    public abstract class BaseHistoryController : Controller
    {
        private readonly IHistoryService _historyService;
        private readonly ILogger _logger;

        public BaseHistoryController(
            IHistoryService historyService,
            ILogger logger)
        {
            _historyService = historyService;
            _logger = logger;
        }

        [HttpGet("{collectionYear}")]
        public async Task<IActionResult> GetHistoriesAsync(int collectionYear)
        {
            try
            {
                var model = await _historyService.GetPeriodHistories(collectionYear);
                return Ok(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        [HttpGet("history-state/{collectionYear}/{period}/{collectionType}")]
        public async Task<IActionResult> GetHistoryStateAsync(int collectionYear, int period, string collectionType, CancellationToken cancellationToken)
        {
            try
            {
                var yearPeriod = new YearPeriod
                {
                    Period = period,
                    Year = collectionYear
                };
                var model = await _historyService.GetPathsHistoryStateAsync(yearPeriod, collectionType, cancellationToken);
                return Ok(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }

        [HttpGet("years")]
        public async Task<IActionResult> GetCollectionYearsAsync()
        {
            try
            {
                var model = await _historyService.GetCollectionYears();
                return Ok(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
        }
    }
}