using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Api.Common;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return Results.Ok(result.Value);

        return result.Error.Code switch
        {
            "Order.NotFound" => Results.NotFound(),

            "Order.ConcurrencyConflict" => Results.Conflict(new ProblemDetails
            {
                Title = "Concurrency conflict",
                Detail = result.Error.Message
            }),

            _ => Results.BadRequest(new ProblemDetails
            {
                Title = "Request failed",
                Detail = result.Error.Message
            })
        };
    }

    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
            return Results.Ok();

        return result.Error.Code switch
        {
            "Order.NotFound" => Results.NotFound(),

            "Order.ConcurrencyConflict" => Results.Conflict(new ProblemDetails
            {
                Title = "Concurrency conflict",
                Detail = result.Error.Message
            }),

            _ => Results.BadRequest(new ProblemDetails
            {
                Title = "Request failed",
                Detail = result.Error.Message
            })
        };
    }
}
