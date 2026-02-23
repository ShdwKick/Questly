
namespace Questly.Middlewares;

public class ResponseSizeLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly long _maxBytes;

    public ResponseSizeLimitMiddleware(RequestDelegate next, long maxBytes = 3 * 1024 * 1024)
    {
        _next = next;
        _maxBytes = maxBytes;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        await _next(context);

        if (buffer.Length > _maxBytes)
        {
            context.Response.StatusCode = StatusCodes.Status413RequestEntityTooLarge;
            // очистить тело и записать сообщение (при необходимости можно логировать)
            context.Response.ContentLength = null;
            buffer.SetLength(0);
            await context.Response.WriteAsync("Response too large");
        }
        else
        {
            buffer.Seek(0, SeekOrigin.Begin);
            await buffer.CopyToAsync(originalBody);
        }

        context.Response.Body = originalBody;
    }
}

public static class ResponseSizeLimitMiddlewareExtensions
{
    public static IApplicationBuilder UseResponseSizeLimit(this IApplicationBuilder app, long maxBytes = 3 * 1024 * 1024)
    {
        return app.UseMiddleware<ResponseSizeLimitMiddleware>(maxBytes);
    }
}