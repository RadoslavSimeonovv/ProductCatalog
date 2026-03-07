using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Order.Entities;
using ProductCatalog.Domain.Payment.Entities;

namespace ProductCatalog.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductFeature> ProductFeatures => Set<ProductFeature>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}