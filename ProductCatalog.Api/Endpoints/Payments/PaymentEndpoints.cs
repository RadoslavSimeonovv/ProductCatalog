using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Common;
using ProductCatalog.Application.Payment;
using ProductCatalog.Application.Payment.CreatePayment;
using ProductCatalog.Application.Payment.GetPaymentById;
using ProductCatalog.Application.Payment.MarkPaymentFailed;
using ProductCatalog.Application.Payment.MarkPaymentSucceeded;

namespace ProductCatalog.Api.Endpoints.Payments;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/payments")
            .WithTags("Payments");

        MapGetPaymentById(group);
        MapCreatePayment(group);
        MapMarkPaymentFailed(group);
        MapMarkPaymentSucceeded(group);

        return app;
    }

    private static void MapGetPaymentById(RouteGroupBuilder group)
    {
        group.MapGet("/{paymentId:guid}", async (
            Guid paymentId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPaymentByIdQuery(paymentId), ct);

            return result.ToHttpResult();
        })
            .WithName("GetPaymentById")
            .WithSummary("Gets a payment by id")
            .Produces<PaymentResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policies.AdminOrCustomer);
    }

    private static void MapCreatePayment(RouteGroupBuilder group)
    {
        group.MapPost("/", async (
            CreatePaymentRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreatePaymentCommand(request.OrderId, 
                request.Provider, request.ProviderReference, request.IdempotencyKey );
            var result = await sender.Send(command, ct);

            if (result.IsSuccess)
                return Results.Created($"/payments/{result.Value}", result.Value);

            return result.ToHttpResult();
        })
            .WithName("CreatePayment")
            .WithSummary("Creates a new payment")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policies.AdminOnly);
    }

    private static void MapMarkPaymentFailed(RouteGroupBuilder group)
    {
        group.MapPost("/{paymentId:guid}/fail", async (
            Guid paymentId,
            MarkPaymentFailedRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new MarkPaymentFailedCommand(paymentId, request.Reason), ct);
            return result.ToHttpResult();
        })
            .WithName("MarkPaymentFailed")
            .WithSummary("Marks a payment as failed")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policies.AdminOnly);
    }

    private static void MapMarkPaymentSucceeded(RouteGroupBuilder group)
    {
        group.MapPost("/{paymentId:guid}/succeed", async (
            Guid paymentId,
            MarkPaymentSucceededRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new MarkPaymentSucceededCommand(paymentId, request.ProviderReference), ct);
            return result.ToHttpResult();
        })
            .WithName("MarkPaymentSucceeded")
            .WithSummary("Marks payment as succeeded")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .RequireAuthorization(Policies.AdminOnly);
    }
}