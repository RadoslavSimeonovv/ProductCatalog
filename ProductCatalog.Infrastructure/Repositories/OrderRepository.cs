using ProductCatalog.Domain.Order.Entities;
using ProductCatalog.Domain.Order.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalog.Infrastructure.Repositories;

internal sealed class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext dbContext) 
        : base(dbContext)
    {
    }

    public override Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext
            .Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<List<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        => _dbContext
            .Orders
            .Include(o => o.Items)
            .ToListAsync(cancellationToken);
}
