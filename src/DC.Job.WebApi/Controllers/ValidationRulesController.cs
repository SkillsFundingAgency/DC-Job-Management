using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/validationrules")]
    public class ValidationRulesController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEnumerable<IValidationRulesService> _validationRulesServices;

        public ValidationRulesController(ILogger logger, IEnumerable<IValidationRulesService> validationRulesServices)
        {
            _logger = logger;
            _validationRulesServices = validationRulesServices;
        }

        [Route("{academicYear}")]
        public async Task<IActionResult> GetRulesAsync(int academicYear, CancellationToken cancellationToken)
        {
            var validationRulesService = _validationRulesServices.Single(s => s.AcademicYear == academicYear);
            var rules = await validationRulesService.GetILRValidationRulesAsync(cancellationToken);
            return Ok(rules);
        }
    }
}
