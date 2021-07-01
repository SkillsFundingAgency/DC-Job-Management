using ESFA.DC.Logging.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ESFA.DC.Job.WebApi.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public ExceptionFilter(ILogger logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var request = context.HttpContext.Request;
            var url = $@"{request.Host.Value}{request.Path}";
            url = request.QueryString.HasValue ? $"{url}{request.QueryString.Value}" : url;
            _logger.LogError($"An error occured in Job API for url: {url}", context.Exception);
        }
    }
}