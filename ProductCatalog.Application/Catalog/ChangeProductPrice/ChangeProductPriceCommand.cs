using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Catalog.ChangeProductPrice;

public sealed record ChangeProductPriceCommand(
    Guid ProductId,
    decimal PriceAmount,
    string Currency) : ICommand;