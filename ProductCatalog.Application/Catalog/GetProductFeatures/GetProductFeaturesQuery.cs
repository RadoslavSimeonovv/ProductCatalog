using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Catalog.GetProductFeatures;

public sealed record GetProductFeaturesQuery(Guid ProductId) 
    : IQuery<GetProductFeaturesQueryResponse>;