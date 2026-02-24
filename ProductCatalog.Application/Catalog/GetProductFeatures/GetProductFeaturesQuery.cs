using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Catalog.GetProductFeatures;

public sealed record GetProductFeaturesQuery(Guid ProductId) 
    : IQuery<GetProductFeaturesQueryResponse>;