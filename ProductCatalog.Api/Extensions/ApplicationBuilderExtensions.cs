using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Middleware;
using ProductCatalog.Infrastructure;

namespace ProductCatalog.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }

    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static void UseRequestContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestContextLoggingMiddleware>();
    }

    public static void UseUserContextLogging(this IApplicationBuilder app)
    {
        app.UseMiddleware<UserContextLoggingMiddleware>();
    }
}