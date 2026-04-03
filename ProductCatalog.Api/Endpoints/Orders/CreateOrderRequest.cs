namespace ProductCatalog.Api.Endpoints.Orders;

public sealed record CreateOrderRequest(List<CreateOrderItemRequest> Items);