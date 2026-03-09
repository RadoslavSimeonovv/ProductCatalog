using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Common;
using ProductCatalog.Application.Order.GetOrderById;
using ProductCatalog.Application.Order.Responses;

namespace ProductCatalog.Api.Endpoints.Orders;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders")
            .WithTags("Orders");

        MapGetOrderById(group);

        return app;
    }

    private static void MapGetOrderById(RouteGroupBuilder group)
    {
        group.MapGet("/{orderId:guid}", async (
            Guid orderId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetOrderByIdQuery(orderId), ct);

            return result.ToHttpResult();
        })
        .WithName("GetOrderById")
        .WithSummary("Gets an order by id")
        .Produces<OrderResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }
}