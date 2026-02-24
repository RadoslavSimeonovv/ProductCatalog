using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Catalog.Responses;

namespace ProductCatalog.Application.Catalog.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) 
    : IQuery<ProductResponse>;