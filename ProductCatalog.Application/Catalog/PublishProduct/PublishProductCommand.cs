using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Catalog.PublishProduct;

public sealed record PublishProductCommand(Guid ProductId) : ICommand;
