using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Enums;
using ProductCatalog.Domain.Order.ValueObjects;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Order.Entities;

public sealed class Order : Entity
{
    public Order(Guid id) : base(id)
    {
    }
    public Email CustomerEmail { get; private set; }
    public Money TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public OrderStatus Status { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
}