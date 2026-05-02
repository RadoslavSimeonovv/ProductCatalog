using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductCatalog.Application.Exceptions;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Order.Entities;
using ProductCatalog.Domain.Payment.Entities;
using ProductCatalog.Infrastructure.Outbox;

namespace ProductCatalog.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

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

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            AddDomainEventsAsOutboxMessages();

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occured.", ex);
        }
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        var outboxMessages = ChangeTracker
             .Entries<Entity>()
             .ToList()
             .Select(e => e.Entity)
             .SelectMany(e =>
             {
                 var domainEvents = e.GetDomainEvents().ToList();

                 e.ClearDomainEvents();

                 return domainEvents;
             })
             .Select(e => new OutboxMessage(
                 Guid.NewGuid(),
                 DateTime.UtcNow,
                 e.GetType().Name,
                 JsonConvert.SerializeObject(e, _jsonSerializerSettings)))
             .ToList();

        AddRange(outboxMessages);
    }
}