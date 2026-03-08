using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Catalog.DeactivateProduct;

public sealed record DeactivateProductCommand(Guid ProductId) : ICommand;