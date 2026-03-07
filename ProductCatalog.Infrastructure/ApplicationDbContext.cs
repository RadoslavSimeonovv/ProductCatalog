using ProductCatalog.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalog.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }
}
