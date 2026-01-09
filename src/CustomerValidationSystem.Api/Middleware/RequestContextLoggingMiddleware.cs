using Serilog.Context;

namespace CustomerValidationSystem.Api.Middleware;

public class RequestContextLoggingMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public RequestContextLoggingMiddleware(RequestDelegate next) =>
        this._next = next;

    public Task Invoke(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Pushes the correlation ID into the Serilog logging context
        using (LogContext.PushProperty("CorrelationId", GetCorrelationId(context)))
        {
            return this._next(context);
        }
    }

    private static string GetCorrelationId(HttpContext context)
    {
        // Tries to get the correlation ID from the header or fallback to trace identifier
        if (context.Request.Headers.TryGetValue(CorrelationIdHeader, out var headerValues))
        {
            return headerValues.FirstOrDefault() ?? context.TraceIdentifier;
        }

        return context.TraceIdentifier;
    }
}
