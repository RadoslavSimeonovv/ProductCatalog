namespace ProductCatalog.Application.Order.GetAllOrders;

internal sealed record OrderRow(
    Guid Id,
    string CustomerId,
    string CustomerEmail,
    string Status,
    Guid? ProductId,
    int? Quantity,
    decimal? UnitPrice,
    string? Currency);
