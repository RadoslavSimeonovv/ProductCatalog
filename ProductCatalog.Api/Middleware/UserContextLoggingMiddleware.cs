using ProductCatalog.Application.Abstractions.Authentication;
using Serilog.Context;

namespace ProductCatalog.Api.Middleware;

public class UserContextLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser)
    {
        if (currentUser.IsAuthenticated)
        {
            using (LogContext.PushProperty("UserId", currentUser.UserId))
            using (LogContext.PushProperty("UserEmail", currentUser.Email))
            {
                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }
    }
}
