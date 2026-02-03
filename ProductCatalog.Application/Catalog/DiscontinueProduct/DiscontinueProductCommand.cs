using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Catalog.DiscontinueProduct;

public sealed record DiscontinueProductCommand(Guid ProductId) : ICommand;