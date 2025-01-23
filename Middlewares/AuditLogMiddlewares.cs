namespace ProjectCore.Middlewares;

public class AuditLogMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger logger;

    public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext c)
    {
        logger.LogInformation($"Hundling requst: {c.Request.Path}.{c.Request.Method}");
        await next(c);
        logger.LogInformation($"Finished hundling requst: {c.Request.Path}.{c.Request.Method} Response status: {c.Response.StatusCode}");
    }
}

public static partial class MiddlewareExtensionsF
{
    public static IApplicationBuilder UseAuditLogMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuditLogMiddleware>();
    }
}