using Microsoft.AspNetCore.Mvc.Filters;

namespace InterceptorDemoApp.Api.Filters
{
    /// <summary>
    /// Custom attribute filter used to log audit trails on specific controllers or actions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuditLoggingAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpContext = context.HttpContext;
            var requestMethod = httpContext.Request.Method;
            var requestPath = httpContext.Request.Path;
            var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

            // Intercept before execution
            Console.WriteLine($"[Audit Interceptor - BEFORE] Method: {requestMethod} | Path: {requestPath} | Client IP: {clientIp}");

            // Add custom tracking item to HttpContext
            httpContext.Items["AuditTimestamp"] = DateTime.UtcNow;

            var executedContext = await next();

            // Intercept after execution
            var statusCode = httpContext.Response.StatusCode;
            Console.WriteLine($"[Audit Interceptor - AFTER] Response Status: {statusCode}");
        }
    }
}
