using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Catalog.UpdateProductFeature;

public sealed record UpdateProductFeatureValueCommand(
    Guid ProductId,
    Guid FeatureId,
    string NewValue) : ICommand;