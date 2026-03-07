using ProductCatalog.Domain.Payment.Entities;
using ProductCatalog.Domain.Payment.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalog.Infrastructure.Repositories;

internal sealed class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext dbContext) 
        : base(dbContext)
    {
    }

    public Task<Payment?> GetByIdempotencyKeyAsync(string key, string provider, CancellationToken cancellationToken = default)
        => _dbContext
            .Payments
            .FirstOrDefaultAsync(p => p.IdempotencyKey == key && p.Provider == provider, cancellationToken);

    public IQueryable<Payment> GetPaymentsByOrderId(Guid orderId)
       => _dbContext
            .Payments
            .AsQueryable()
            .Where(p  => p.OrderId == orderId);
}
