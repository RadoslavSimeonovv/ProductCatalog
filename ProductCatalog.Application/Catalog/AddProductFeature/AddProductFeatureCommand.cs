using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Catalog.AddProductFeature;

public sealed record AddProductFeatureCommand(
    Guid ProductId, Guid FeatureId, string Name, string Value, int DisplayOrder)
    : ICommand<Guid>;