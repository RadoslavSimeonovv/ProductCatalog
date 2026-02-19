namespace ProductCatalog.Application.Order.Responses;

public sealed class OrderResponse
{
    public Guid Id { get; init; }
    public required string CustomerEmail { get; init; }
    public required string Status { get; init; }
    public required List<OrderItemResponse> Items { get; init; }
}
