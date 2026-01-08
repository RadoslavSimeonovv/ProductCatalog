using PlanFlow.Domain.Catalog.Entities;
using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Order.Entities;

public sealed class OrderItem : Entity
{
    public OrderItem(Guid id) : base(id)
    {
    }

    public Order Order { get; private set; }
    public Guid OrderId { get; private set; }

    public Product Product { get; private set; }
    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public Money UnitPrice { get; private set; }

    public Money UnitTotal { get; private set; }
}
