using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Catalog.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) 
    : IQuery<ProductResponse>;