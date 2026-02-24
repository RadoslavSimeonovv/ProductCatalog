using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Catalog.Responses;

namespace ProductCatalog.Application.Catalog.GetAllCategories;

public sealed record GetAllCategoriesQuery 
    : IQuery<IReadOnlyList<ProductCategoryResponse>>;