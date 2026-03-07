using ProductCatalog.Domain.Catalog.Entities;

namespace ProductCatalog.Domain.Catalog.Repositories;

public interface IProductCategoryRepository
{
    Task<ProductCategory?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<ICollection<ProductCategory>> GetAllAsync(CancellationToken cancellationToken = default);
    IQueryable<ProductCategory> Query();
    void Add(ProductCategory category);
}
