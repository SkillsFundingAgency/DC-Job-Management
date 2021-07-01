using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.JobQueueManager.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/returnperiod")]
    public class ReturnPeriodController : Controller
    {
        private IReturnPeriodService _returnPeriodService;

        public ReturnPeriodController(IReturnPeriodService returnPeriodService)
        {
            _returnPeriodService = returnPeriodService;
        }

        [HttpGet("collectionId/{collectionId}")]
        public async Task<IActionResult> GetReturnPeriodsForCollectionAsync(int collectionId)
        {
            var returnPeriods = await _returnPeriodService.GetReturnPeriodsForCollectionAsync(collectionId);

            if (returnPeriods.Any())
            {
                return Ok(returnPeriods);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("upto/{givenDateUtc}/{collectionType}")]
        public async Task<IActionResult> GetReturnPeriodsUpToGivenDateForCollectionTypeAsync(DateTime givenDateUtc, string collectionType, CancellationToken cancellationToken)
        {
            var maxPeriod = await _returnPeriodService.GetReturnPeriodsUpToGivenDateForCollectionTypeAsync(givenDateUtc, collectionType, cancellationToken);

            return Ok(maxPeriod);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var returnPeriod = await _returnPeriodService.GetReturnPeriodAsync(id);

            if (returnPeriod != null)
            {
                return Ok(returnPeriod);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPost("update")]
        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        public async Task<IActionResult> UpdateAsync([FromBody] ReturnPeriod returnPeriod, CancellationToken cancellationToken)
        {
            var isSuccess = await _returnPeriodService.UpdateReturnPeriod(returnPeriod, cancellationToken);

            if (isSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}