using ProductCatalog.Domain.Catalog.Entities;

namespace ProductCatalog.UnitTests.Domain.Catalog;

internal static class ProductCategoryData
{
    public static readonly string Name = "Test Category";
    public static readonly string Description = "Test Description";

    public static readonly string InvalidName = "";

    public static ProductCategory CreateCategory() =>
        ProductCategory.Create(Name, Description).Value;
}
