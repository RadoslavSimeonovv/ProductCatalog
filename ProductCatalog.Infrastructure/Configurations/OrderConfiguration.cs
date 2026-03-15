using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Order.Entities;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Infrastructure.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId)
            .HasConversion(
                id => id.Value,
                value => CustomerId.FromDb(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CustomerEmail)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.OwnsOne(x => x.TotalAmount, totalBuilder =>
        {
            totalBuilder.Property(p => p.Amount)
                .HasColumnName("total_amount")
                .HasPrecision(18, 2)
                .IsRequired();

            totalBuilder.Property(p => p.Currency)
                .HasConversion(
                    c => c.Code,
                    code => Currency.FromCode(code))
                .HasColumnName("total_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Navigation(x => x.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);

        builder.Property(x => x.Version)
            .IsRowVersion();
    }
}
