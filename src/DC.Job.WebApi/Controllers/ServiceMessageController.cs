using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Job.WebApi.Filters;
using ESFA.DC.JobQueueManager.Interfaces;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/service-message")]
    public class ServiceMessageController : ControllerBase
    {
        private readonly IServiceMessageService _serviceMessageService;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;

        public ServiceMessageController(
            IServiceMessageService serviceMessageService,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
        {
            _serviceMessageService = serviceMessageService;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        [HttpGet("{controllerName?}")]
        public async Task<IActionResult> GetAsync(string controllerName, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Get request received to fetch service message");

            if (controllerName == null)
            {
                controllerName = string.Empty;
            }

            try
            {
                var message = await _serviceMessageService.GetMessageAsync(_dateTimeProvider.GetNowUtc(), controllerName, cancellationToken);
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred trying to fetch service message", ex);
                return new BadRequestResult();
            }
        }

        [HttpGet("pages")]
        public async Task<IActionResult> GetAllPagesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo("Get request received to fetch service message");

            try
            {
                var message = await _serviceMessageService.GetAllPagesAsync(cancellationToken);
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred trying to fetch service pages", ex);
                return new BadRequestResult();
            }
        }

        [HttpGet("id/{id:int}")]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken, int id)
        {
            try
            {
                var message = await _serviceMessageService.GetMessageAsync(id, cancellationToken);
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred trying to fetch service message", ex);
                return new BadRequestResult();
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(CancellationToken cancellationToken, int id)
        {
            try
            {
                await _serviceMessageService.DeleteMessageAsync(id, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred trying to delete service message", ex);
                return new BadRequestResult();
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllMessagesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo("Get request received to fetch all service messages");

            try
            {
                var message = await _serviceMessageService.GetAllMessagesAsync(cancellationToken);
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred trying to fetch all service messages", ex);
                return new BadRequestResult();
            }
        }

        [HttpGet("controllers")]
        public async Task<IActionResult> GetControllerNamesThatDisplayMessagesAsync(CancellationToken cancellationToken)
        {
            _logger.LogInfo("Get request received to fetch all controllers that should display a message");

            try
            {
                var message = await _serviceMessageService.GetControllerNamesThatDisplayMessagesAsync(cancellationToken);
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred trying to fetch all controllers that should display a message", ex);
                return new BadRequestResult();
            }
        }

        [ServiceFilter(typeof(AuditFilter), IsReusable = false)]
        [HttpPost]
        public async Task<IActionResult> SaveAsync(CancellationToken cancellationToken, [FromBody] ServiceMessageDto dto)
        {
            try
            {
                var message = await _serviceMessageService.SaveMessageAsync(dto, cancellationToken);
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred trying to save service message", ex);
                return new BadRequestResult();
            }
        }
    }
}