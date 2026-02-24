using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Order.CancelOrder;

public sealed record CancelOrderCommand(
    Guid OrderId, 
    string Reason) : ICommand;