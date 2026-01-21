using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Enums;
using ProductCatalog.Domain.Order.Events;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Order.Entities;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _items;
    private Order(Guid id,
        string customerEmail,
        Money totalAmount,
        List<OrderItem> items,
        DateTime createdAt) : base(id)
    {
        CustomerEmail = customerEmail;
        TotalAmount = totalAmount;
        Status = OrderStatus.Created;
        _items = [.. items];
        CreatedAt = createdAt;
    }
    public string CustomerEmail { get; private set; }
    public Money TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(string customerEmail, List<OrderItem> items)
    {
        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Customer email is required.", nameof(customerEmail));

        if (items is null)
            throw new ArgumentNullException(nameof(items));

        if (items.Count == 0)
            throw new InvalidOperationException("Order must contain at least one item.");

        var dateTimeNow = DateTime.UtcNow;

        var orderItems = new List<OrderItem>(items);
        var orderCurrency = items[0].LineTotal.Currency;
        var orderTotal = Money.Zero(orderCurrency);

        foreach (var item in orderItems)
        {
            if (item.LineTotal.Currency != orderCurrency)
                throw new InvalidOperationException("All order items must have the same currency.");

            orderTotal += item.LineTotal;
        }

        var order = new Order(Guid.NewGuid(),
            customerEmail,
            orderTotal,
            items,
            dateTimeNow);

        order.RaiseDomainEvent(new OrderCreatedDomainEvent(
            order.Id, order.CustomerEmail, order.TotalAmount, dateTimeNow));

        return order;
    }
}