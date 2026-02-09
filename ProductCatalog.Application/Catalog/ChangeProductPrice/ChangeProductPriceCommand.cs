using ProductCatalog.Application.Messaging;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Application.Catalog.ChangeProductPrice;

public sealed record ChangeProductPriceCommand(
    Guid ProductId,
    Money NewPrice) : ICommand;