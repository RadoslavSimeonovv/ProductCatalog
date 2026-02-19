namespace ProductCatalog.Application.Order;

public sealed record CreateOrderItemDto(
    Guid ProductId,
    int Quantity,
    decimal UnitPriceAmount,
    string Currency
);