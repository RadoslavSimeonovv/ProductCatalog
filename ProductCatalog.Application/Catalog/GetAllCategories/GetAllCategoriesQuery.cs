using ProductCatalog.Application.Catalog.Responses;
using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Catalog.GetAllCategories;

public sealed record GetAllCategoriesQuery 
    : IQuery<IReadOnlyList<ProductCategoryResponse>>;