namespace ProductCatalog.Application.Order.Responses;

public sealed record OrderItemResponse(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice,
    string Currency);
