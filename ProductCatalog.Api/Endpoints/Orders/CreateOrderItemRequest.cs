namespace ProductCatalog.Api.Endpoints.Orders;

public sealed record CreateOrderItemRequest(Guid ProductId,
    int Quantity,
    decimal UnitPriceAmount,
    string Currency);