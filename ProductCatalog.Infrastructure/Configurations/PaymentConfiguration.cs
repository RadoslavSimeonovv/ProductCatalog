using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductCatalog.Domain.Payment.Entities;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Infrastructure.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.CustomerId)
            .HasConversion(
                id => id.Value,
                value => CustomerId.FromDb(value))
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.Provider)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ProviderReference)
            .HasMaxLength(100);

        builder.Property(x => x.IdempotencyKey)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.OwnsOne(x => x.Amount, amountBuilder =>
        {
            amountBuilder.Property(p => p.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            amountBuilder.Property(p => p.Currency)
                .HasConversion(
                    c => c.Code,
                    code => Currency.FromCode(code))
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.CustomerId);
        builder.HasIndex(x => new { x.Provider, x.IdempotencyKey }).IsUnique();

    }
}
