namespace ProductCatalog.Application.Order.Responses;

public sealed class OrderItemResponse
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public required string Currency { get; init; }
}