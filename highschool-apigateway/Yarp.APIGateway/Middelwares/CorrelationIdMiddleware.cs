namespace Yarp.APIGateway.Middelwares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("X-Correlation-ID"))
        {
            context.Request.Headers["X-Correlation-ID"] = Guid.NewGuid().ToString();
        }
        await _next(context);
    }
}