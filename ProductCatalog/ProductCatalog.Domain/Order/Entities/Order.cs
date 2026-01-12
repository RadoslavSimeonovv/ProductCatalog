using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Enums;
using ProductCatalog.Domain.Order.ValueObjects;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Order.Entities;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _items = new();
    private Order(Guid id,
        Email customerEmail,
        Money totalAmount) : base(id)
    {
        CustomerEmail = customerEmail;
        TotalAmount = totalAmount;
        Status = OrderStatus.Created;
    }
    public Email CustomerEmail { get; private set; }
    public Money TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(Email customerEmail,
        Money totalAmount)
    {
        return new Order(Guid.NewGuid(), customerEmail, totalAmount);
    }
}