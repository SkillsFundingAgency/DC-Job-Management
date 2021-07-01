using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.PeriodEnd.Interfaces;
using ESFA.DC.PeriodEnd.Interfaces.DataAccess;
using ESFA.DC.PeriodEnd.Models;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers.PeriodEnd
{
    [Produces("application/json")]
    [Route("api/validityperiod")]
    public class ValidityPeriodController : Controller
    {
        private readonly IValidityPeriodRepository _validityPeriodRepository;
        private readonly IValidityPeriodService _validityPeriodService;
        private readonly IValidityStructureService _validityStructureService;

        public ValidityPeriodController(
            IValidityPeriodRepository validityPerionRepository,
            IValidityPeriodService validityPeriodService,
            IValidityStructureService validityStructureService)
        {
            _validityPeriodRepository = validityPerionRepository;
            _validityPeriodService = validityPeriodService;
            _validityStructureService = validityStructureService;
        }

        [HttpGet("validityperiodlist")]
        public async Task<IActionResult> GetValidityPeriodList(CancellationToken cancellationToken)
        {
            return Ok(await _validityPeriodRepository.GetValidityPeriodList(cancellationToken));
        }

        [HttpPut("updatevalidityperiod")]
        public async Task<IActionResult> UpdateValidityPeriod([FromBody] ValidityPeriodLookupModel command, CancellationToken cancellationToken)
        {
            return Ok(await _validityPeriodRepository.UpdateValidityPeriod(command, cancellationToken));
        }

        [HttpPut("updatesubpathvalidityperiods")]
        public async Task<IActionResult> UpdateSubPathValidityPeriods([FromBody] List<SubPathValidityPeriodLookupModel> periods, CancellationToken cancellationToken)
        {
            await _validityPeriodRepository.UpdateSubPathValidityPeriods(periods, cancellationToken);
            return Ok();
        }

        [HttpGet("{CollectionId}/{CollectionYear}/{Period}")]
        public async Task<IActionResult> GetValidityPeriod(int collectionId, int collectionYear, int period, CancellationToken cancellationToken)
        {
            return Ok(await _validityPeriodRepository.GetValidityPeriod(collectionId, collectionYear, period, cancellationToken));
        }

        [HttpGet("validityperiodlist/{CollectionYear}/{Period}")]
        public async Task<IActionResult> GetValidityPeriodList(int collectionYear, int period, CancellationToken cancellationToken)
        {
            return Ok(await _validityPeriodRepository.GetValidityPeriodList(collectionYear, period, cancellationToken));
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPut("updatevalidityperiods/{CollectionYear}/{Period}")]
        public async Task<IActionResult> UpdateValidityPeriod(
            int collectionYear,
            int period,
            [FromBody] List<ValidityPeriodModel> validityPeriods,
            CancellationToken cancellationToken)
        {
            await _validityPeriodService.UpdateValidityPeriods(collectionYear, period, validityPeriods, cancellationToken);
            return Ok();
        }

        [HttpGet("allvaliditiesperperiod/{CollectionYear}/{Period}")]
        public async Task<IActionResult> GetAllEntitiesWithValidityForPeriod(int collectionYear, int period, CancellationToken cancellationToken)
        {
            var yearPeriod = new YearPeriod
            {
                Period = period,
                Year = collectionYear
            };

            return Ok(await _validityStructureService.GetAllPeriodEndItems(yearPeriod, cancellationToken));
        }
    }
}
