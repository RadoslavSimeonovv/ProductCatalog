using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Catalog.RemoveProductFeature;

public sealed record RemoveProductFeatureCommand(
    Guid ProductId,
    Guid FeatureId) : ICommand;