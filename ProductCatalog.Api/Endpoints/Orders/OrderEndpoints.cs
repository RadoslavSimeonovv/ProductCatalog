using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Common;
using ProductCatalog.Application.Order;
using ProductCatalog.Application.Order.CancelOrder;
using ProductCatalog.Application.Order.CreateOrder;
using ProductCatalog.Application.Order.GetAllOrders;
using ProductCatalog.Application.Order.GetOrderById;
using ProductCatalog.Application.Order.Responses;
using ProductCatalog.Application.Payment;
using ProductCatalog.Application.Payment.GetPaymentsByOrderId;

namespace ProductCatalog.Api.Endpoints.Orders;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/orders")
            .WithTags("Orders");

        MapGetOrderById(group);
        MapGetAllOrders(group);
        MapCreateOrder(group);
        MapCancelOrder(group);
        MapGetPaymentsByOrderId(group);

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
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static void MapCreateOrder(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreateOrderRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateOrderCommand(
                request.CustomerEmail, 
                request.CustomerId,
                request.Items.Select(i => new CreateOrderItemDto(
                i.ProductId,
                i.Quantity,
                i.UnitPriceAmount,
                i.Currency)).ToList());

            var result = await sender.Send(command, ct);

            if (result.IsSuccess)
                return Results.Created($"/orders/{result.Value}", result.Value);

            return result.ToHttpResult();
        })
            .WithName("CreateOrder")
            .WithSummary("Creates a new order")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapGetAllOrders(RouteGroupBuilder group)
    {
        group.MapGet("/", async (
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllOrdersQuery(), ct);
            return result.ToHttpResult();
        })
            .WithName("GetAllOrders")
            .WithSummary("Gets all orders")
            .Produces<List<OrderResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapCancelOrder(RouteGroupBuilder group)
    {
        group.MapPost("/{orderId:guid}/cancel", async (
            Guid orderId,
            CancelOrderRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new CancelOrderCommand(orderId, request.Reason!), ct);
            return result.ToHttpResult();
        })
            .WithName("CancelOrder")
            .WithSummary("Cancels an order")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static void MapGetPaymentsByOrderId(RouteGroupBuilder group)
    {
        group.MapGet("/{orderId:guid}/payments", async (
            Guid orderId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPaymentsByOrderIdQuery(orderId), ct);

            return result.ToHttpResult();
        })
            .WithName("GetPaymentsByOrderId")
            .WithSummary("Get payments by order id")
            .Produces<IReadOnlyList<PaymentResponse>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }
}