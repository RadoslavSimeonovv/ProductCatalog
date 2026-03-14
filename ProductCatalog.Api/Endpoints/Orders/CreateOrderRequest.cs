namespace ProductCatalog.Api.Endpoints.Orders;

public sealed record CreateOrderRequest(string CustomerEmail,
    string CustomerId,
    List<CreateOrderItemRequest> Items);