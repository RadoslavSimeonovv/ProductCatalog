using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Entities;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Order.Entities;

public sealed class OrderItem : Entity
{
    public OrderItem(Guid id,
        Guid orderId,
        Guid productId,
        int quantity,
        Money unitPrice) : base(id)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("OrderId cannot be empty.", nameof(orderId));

        if (productId == Guid.Empty)
            throw new ArgumentException("ProductId cannot be empty.", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");

        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));

        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;

        LineTotal = unitPrice * quantity;
    }

    public Order? Order { get; private set; }
    public Guid OrderId { get; private set; }

    public Product? Product { get; private set; }
    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public Money UnitPrice { get; private set; }

    public Money LineTotal { get; private set; }
}