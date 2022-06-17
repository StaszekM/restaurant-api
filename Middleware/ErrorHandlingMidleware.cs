using RestaurantApi.Exceptions;
namespace RestaurantApi.Middleware;

public class ErrorHandlingMiddleware : IMiddleware
{
    private ILogger<ErrorHandlingMiddleware> _logger;
    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (ForbiddenException)
        {
            context.Response.StatusCode = 403;
        }
        catch (NotFoundException nfe)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(nfe.Message);
        }
        catch (BadRequestException bre)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(bre.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Something went wrong");
        }
    }
}