namespace Accounts.Api.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            httpContext.Response.ContentType = MediaTypeNames.Text.Plain;

            (httpContext.Response.StatusCode, var message) = e switch
            {
                EntityNotFoundException => (StatusCodes.Status404NotFound, e.Message),

                DomainException or
                Infrastructure.DuplicateKeyException or
                DuplicateRequestException => (StatusCodes.Status409Conflict, e.Message),

                _ => (StatusCodes.Status500InternalServerError, "Internal server errror.")
            };

            await httpContext.Response.WriteAsync(message);
        }
    }
}