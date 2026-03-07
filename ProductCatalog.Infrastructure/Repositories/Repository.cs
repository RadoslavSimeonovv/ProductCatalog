using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Infrastructure.Repositories;

public abstract class Repository<T>
    where T : Entity
{
    protected readonly ApplicationDbContext _dbContext;

    protected Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(
        Guid Id, 
        CancellationToken cancellationToken = default)
        => await _dbContext
            .Set<T>()
            .FirstOrDefaultAsync(e => e.Id == Id, cancellationToken);

    public void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }
}