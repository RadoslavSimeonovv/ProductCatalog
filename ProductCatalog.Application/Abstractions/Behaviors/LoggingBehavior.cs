using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductCatalog.Domain.Abstractions;
using Serilog.Context;

namespace ProductCatalog.Application.Abstractions.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseRequest
    where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var name = request.GetType().Name;
        var stopwatch = Stopwatch.StartNew();

        using (_logger.BeginScope("{Request}", name))
        {
            try
            {
                _logger.LogInformation("Executing request: {Request}", name);

                var result = await next();

                stopwatch.Stop();

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Request executed successfully: {Request} ({ElapsedMs}ms)", name, stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    using (LogContext.PushProperty("Error", result.Error, true))
                    {
                        _logger.LogWarning("Request executed with errors: {Request} ({ElapsedMs}ms)", name, stopwatch.ElapsedMilliseconds);
                    }
                }

                return result;
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                _logger.LogError(exception, "Error executing request: {Request} ({ElapsedMs}ms)", name, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
