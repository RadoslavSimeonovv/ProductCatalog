using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Catalog.Responses;

namespace ProductCatalog.Application.Catalog.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id)
    : IQuery<ProductCategoryResponse>;
