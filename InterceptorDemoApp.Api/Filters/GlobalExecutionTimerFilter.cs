using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace InterceptorDemoApp.Api.Filters
{
    /// <summary>
    /// A global action filter (interceptor) that tracks request execution duration,
    /// logs latency metrics, and attaches execution telemetry headers to the HTTP response.
    /// </summary>
    public sealed class GlobalExecutionTimerFilter : IAsyncActionFilter
    {
        private const string ElapsedHeaderKey = "X-Response-Time-Ms";
        private readonly ILogger<GlobalExecutionTimerFilter> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExecutionTimerFilter"/> class.
        /// </summary>
        /// <param name="logger">The logger instance injected via dependency injection.</param>
        public GlobalExecutionTimerFilter(ILogger<GlobalExecutionTimerFilter> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Intercepts the controller action invocation lifecycle both before and after execution.
        /// </summary>
        /// <param name="context">The action executing context containing request metadata and pipeline information.</param>
        /// <param name="next">The delegate representing the subsequent action filter or the controller action itself.</param>
        /// <returns>A task representing the asynchronous interception operation.</returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // -------------------------------------------------------------
            // PRE-EXECUTION (Before the controller action runs)
            // -------------------------------------------------------------
            var actionDescriptor = context.ActionDescriptor.DisplayName;
            var httpMethod = context.HttpContext.Request.Method;
            var requestPath = context.HttpContext.Request.Path;
            var correlationId = context.HttpContext.TraceIdentifier;

            _logger.LogInformation(
                "[{CorrelationId}] [START] Intercepting {HttpMethod} {RequestPath} (Action: {Action})",
                correlationId,
                httpMethod,
                requestPath,
                actionDescriptor);

            var stopwatch = Stopwatch.StartNew();

            // -------------------------------------------------------------
            // ACTION EXECUTION (Downstream pipeline execution)
            // -------------------------------------------------------------
            var executedContext = await next();

            // -------------------------------------------------------------
            // POST-EXECUTION (After the controller action runs)
            // -------------------------------------------------------------
            stopwatch.Stop();
            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            // Append custom execution metric header to response if headers are not already sent
            if (!context.HttpContext.Response.HasStarted)
            {
                context.HttpContext.Response.Headers.TryAdd(ElapsedHeaderKey, elapsedMilliseconds.ToString());
            }

            // Handle and log execution status
            if (executedContext.Exception is not null && !executedContext.ExceptionHandled)
            {
                _logger.LogError(
                    executedContext.Exception,
                    "[{CorrelationId}] [FAILED] Action {Action} threw an unhandled exception after {ElapsedMs} ms",
                    correlationId,
                    actionDescriptor,
                    elapsedMilliseconds);
            }
            else
            {
                const long slowThresholdMs = 500;

                if (elapsedMilliseconds > slowThresholdMs)
                {
                    _logger.LogWarning(
                        "[{CorrelationId}] [SLOW ACTION] Action {Action} exceeded threshold ({ThresholdMs} ms) taking {ElapsedMs} ms",
                        correlationId,
                        actionDescriptor,
                        slowThresholdMs,
                        elapsedMilliseconds);
                }
                else
                {
                    _logger.LogInformation(
                        "[{CorrelationId}] [END] Action {Action} completed in {ElapsedMs} ms",
                        correlationId,
                        actionDescriptor,
                        elapsedMilliseconds);
                }
            }
        }
    }
}
