using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Catalog.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id)
    : IQuery<ProductCategoryResponse>;
