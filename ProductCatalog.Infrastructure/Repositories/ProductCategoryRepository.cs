using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Repositories;

namespace ProductCatalog.Infrastructure.Repositories;

internal sealed class ProductCategoryRepository : Repository<ProductCategory>, IProductCategoryRepository
{
    public ProductCategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public IQueryable<ProductCategory> Query()
        => _dbContext
            .ProductCategories
            .AsNoTracking()
            .AsQueryable();
}