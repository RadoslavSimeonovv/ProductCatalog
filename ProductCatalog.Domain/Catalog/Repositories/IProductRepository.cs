using ProductCatalog.Domain.Catalog.Entities;

namespace ProductCatalog.Domain.Catalog.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    IQueryable<Product> Query();
    IQueryable<Product> QueryWithCategory();
    void Add(Product product);
    void MarkFeatureAsAdded(ProductFeature feature);
}