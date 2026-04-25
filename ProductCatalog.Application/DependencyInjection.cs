using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Application.Abstractions.Behaviors;

namespace ProductCatalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));

            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

            cfg.AddOpenBehavior(typeof(QueryCachingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}
