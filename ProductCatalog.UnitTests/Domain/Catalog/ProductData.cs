using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.ValueObjects;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.UnitTests.Domain.Catalog;

internal static class ProductData
{
    public static readonly string Name = "Test Product";
    public static readonly string Description = "Test Description";
    public static readonly Money Price = new Money(9.99m, new Currency("USD"));
    public static readonly Money NewPrice = new Money(19.99m, new Currency("USD"));
    public static readonly Guid CategoryId = new Guid("a1b2c3d4-0000-0000-0000-000000000001");
    public static readonly Guid NewCategoryId = new Guid("a1b2c3d4-0000-0000-0000-000000000002");
    public static readonly Sku Sku = Sku.Create("TEST-SKU-001").Value;

    public static readonly string InvalidName = "";

    public static readonly Guid FeatureId = new Guid("f1f2f3f4-0000-0000-0000-000000000001");
    public static readonly string FeatureName = "Color";
    public static readonly string FeatureValue = "Red";
    public static readonly string NewFeatureValue = "Blue";
    public static readonly int FeatureDisplayOrder = 1;

    public static Product CreateProduct() =>
        Product.Create(Name, Description, Price, CategoryId, Sku).Value;

    public static Product CreateActiveProduct()
    {
        var product = CreateProduct();
        product.Publish();
        return product;
    }

    public static Product CreateDiscontinuedProduct()
    {
        var product = CreateProduct();
        product.Discontinue();
        return product;
    }

    public static Product CreateProductWithFeature()
    {
        var product = CreateProduct();
        product.AddFeature(FeatureId, FeatureName, FeatureValue, FeatureDisplayOrder);
        return product;
    }
}
