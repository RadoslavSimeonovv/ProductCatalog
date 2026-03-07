using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Order.Entities;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Infrastructure.Configurations;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.ProductId)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.OwnsOne(x => x.UnitPrice, unitPriceBuilder =>
        {
            unitPriceBuilder.Property(p => p.Amount)
                .HasColumnName("UnitPriceAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            unitPriceBuilder.Property(p => p.Currency)
                .HasConversion(
                    c => c.Code,
                    code => Currency.FromCode(code))
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(x => x.LineTotal, lineTotalBuilder =>
        {
            lineTotalBuilder.Property(p => p.Amount)
                .HasColumnName("LineTotalAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            lineTotalBuilder.Property(p => p.Currency)
                .HasConversion(
                    c => c.Code,
                    code => Currency.FromCode(code))
                .HasColumnName("LineTotalCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasOne(x => x.Order)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.ProductId);

    }
}
