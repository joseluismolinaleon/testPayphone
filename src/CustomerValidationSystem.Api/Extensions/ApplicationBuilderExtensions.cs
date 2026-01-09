using CustomerValidationSystem.Api.Middleware;

namespace CustomerValidationSystem.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds global exception handling middleware to the request pipeline.
    /// </summary>
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Adds request context logging middleware to enrich logs with correlation ID.
    /// </summary>
    public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<RequestContextLoggingMiddleware>();
    }
}
