using System.Diagnostics;

namespace RestaurantApi.Middleware;

public class TimeTrackingMiddleware : IMiddleware
{
    private ILogger<TimeTrackingMiddleware> _logger;
    public TimeTrackingMiddleware(ILogger<TimeTrackingMiddleware> logger)
    {
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Stopwatch watch = new Stopwatch();
        watch.Start();
        await next.Invoke(context);
        watch.Stop();

        if (watch.ElapsedMilliseconds > 4000)
        {
            _logger.LogTrace($"{context.Request.Method} request for path {context.Request.Path} took {watch.ElapsedMilliseconds}ms");
        }
    }
}