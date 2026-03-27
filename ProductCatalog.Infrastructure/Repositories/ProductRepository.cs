using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Catalog.Repositories;

namespace ProductCatalog.Infrastructure.Repositories;

internal sealed class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public IQueryable<Product> Query()
        => _dbContext
            .Products
            .AsNoTracking()
            .AsQueryable();

    public IQueryable<Product> QueryWithCategory()
        => _dbContext
            .Products
            .Include(p => p.Category)
            .AsNoTracking()
            .AsQueryable();

    public override async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbContext
            .Products
            .Include(p => p.Features)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public void MarkFeatureAsAdded(ProductFeature feature)
    {
        _dbContext.Entry(feature).State = EntityState.Added;
    }
}