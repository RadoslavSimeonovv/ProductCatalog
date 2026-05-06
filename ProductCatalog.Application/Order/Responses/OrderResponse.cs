namespace ProductCatalog.Application.Order.Responses;

public sealed record OrderResponse(
    Guid Id,
    string CustomerId,
    string CustomerEmail,
    string Status,
    IReadOnlyList<OrderItemResponse> Items);
