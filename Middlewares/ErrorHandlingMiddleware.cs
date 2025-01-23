namespace ProjectCore.Middlewares;

public class ErrorHandlingMiddleware
{
    private RequestDelegate next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task Invoke(HttpContext c)
    {
        try
        {
            await next(c);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(c, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext Context, Exception exception)
    {
        Context.Response.ContentType = "application/json";
        Context.Response.StatusCode = 500;
        var response = new
        {
            message = "An unexpected error occurred.",
            details = exception.Message
        };
        return Context.Response.WriteAsJsonAsync(response);
    }
}

public static partial class MiddlewareExtensionsF
{
    public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
