using System;
using ESFA.DC.JobQueueManager.Interfaces.Audit;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ESFA.DC.Job.WebApi.Filters
{
    public class AuditFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;
            headers.TryGetValue("AuditUsername", out var username);

            int? differentiator = null;
            DateTime? timestamp = null;

            if (headers.TryGetValue("AuditDifferentiator", out var differentiatorValue))
            {
                if (int.TryParse(differentiatorValue, out var differentiatorInt))
                {
                    differentiator = differentiatorInt;
                }
            }

            if (headers.TryGetValue("AuditDateTime", out var timeStampValue))
            {
                if (DateTime.TryParse(timeStampValue, out var timeStampDateTime))
                {
                    timestamp = timeStampDateTime;
                }
            }

            var auditContextProvider = context.HttpContext.RequestServices.GetService(typeof(IAuditContextProvider)) as IAuditContextProvider;

            auditContextProvider.Configure(username, differentiator, timestamp);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
