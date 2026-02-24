using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Catalog.RemoveProductFeature;

public sealed record RemoveProductFeatureCommand(
    Guid ProductId,
    Guid FeatureId) : ICommand;