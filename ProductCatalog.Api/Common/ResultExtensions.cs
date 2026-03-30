using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Api.Common;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
        => result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToHttpResult();

    public static IResult ToHttpResult(this Result result)
        => result.IsSuccess
            ? Results.NoContent()
            : result.Error.ToHttpResult();

    private static IResult ToHttpResult(this Error error) =>
        error.Type switch
        {
            ErrorType.NotFound => Results.NotFound(),
            ErrorType.Unauthorized => Results.Forbid(),
            ErrorType.Conflict => Results.Conflict(new ProblemDetails
            {
                Title = "Conflict",
                Detail = error.Message
            }),
            ErrorType.Validation => Results.UnprocessableEntity(new ProblemDetails
            {
                Title = "Validation error",
                Detail = error.Message
            }),
            _ => Results.BadRequest(new ProblemDetails
            {
                Title = "Request failed",
                Detail = error.Message
            })
        };
}