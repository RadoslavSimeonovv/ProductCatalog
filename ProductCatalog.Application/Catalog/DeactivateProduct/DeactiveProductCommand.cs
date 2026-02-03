using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Catalog.DeactivateProduct;

public sealed record DeactiveProductCommand(Guid ProductId) : ICommand;