using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.Exceptions;

namespace ProductCatalog.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            if (context.Response.HasStarted)
                throw;

            var exceptionDetails = GetExceptionDetails(exception);

            var problemDetails = new ProblemDetails
            {
                Status = exceptionDetails.Status,
                Type = exceptionDetails.Type,
                Title = exceptionDetails.Title,
                Detail = exceptionDetails.Detail,
            };

            problemDetails.Extensions["traceId"] = context.TraceIdentifier;

            if (exceptionDetails.Errors is not null)
            {
                problemDetails.Extensions["errors"] = exceptionDetails.Errors;
            }

            context.Response.StatusCode = exceptionDetails.Status;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    private static ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => new ExceptionDetails(
                StatusCodes.Status422UnprocessableEntity,
                "https://httpstatuses.com/422",
                "Validation error",
                "One or more validation errors occurred.",
                validationException.Errors),

            ConcurrencyException => new ExceptionDetails(
                StatusCodes.Status409Conflict,
                "https://httpstatuses.com/409",
                "Concurrency conflict",
                "The resource was modified by another process.",
                null),

            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                "https://httpstatuses.com/500",
                "Server error",
                "An unexpected error has occurred.",
                null)
        };
    }

    internal sealed record ExceptionDetails(
        int Status,
        string Type,
        string Title,
        string Detail,
        IEnumerable<object>? Errors);
}
