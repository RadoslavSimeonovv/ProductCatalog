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

    /// <summary>
    /// Factory method to create a new Order
    /// </summary>
    /// <param name="customerEmail"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
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

    /// <summary>
    /// Submits the order for payment processing by transitioning its status to awaiting payment.
    /// </summary>
    /// <remarks>This method should be called only once per order, after it has been created and before
    /// payment is initiated. Upon successful submission, a domain event is raised to signal that the order is ready for
    /// payment.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the order status is not <see cref="OrderStatus.Created"/>.</exception>
    public void SubmitForPayment()
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Only created orders can be submitted for payment.");

        if (_items.Count == 0)
            throw new InvalidOperationException("Order must contain at least one item.");

        var dateTimeNow = DateTime.UtcNow;
        Status = OrderStatus.AwaitingPayment;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new OrderSubmittedForPaymentDomainEvent(
            Id, TotalAmount, dateTimeNow));
    }

    /// <summary>
    /// Marks the order as paid if it is currently awaiting payment.
    /// </summary>
    /// <remarks>This method updates the order status to <see cref="OrderStatus.Paid"/> and records the
    /// payment time. A domain event is raised to signal that the order has been paid.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the order status is not <see cref="OrderStatus.AwaitingPayment"/>.</exception>
    public void MarkAsPaid()
    {
        if (Status != OrderStatus.AwaitingPayment)
            throw new InvalidOperationException("Only orders awaiting payment can be marked as paid.");

        var dateTimeNow = DateTime.UtcNow;
        Status = OrderStatus.Paid;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new OrderPaidDomainEvent(
            Id, TotalAmount, dateTimeNow));
    }

    /// <summary>
    /// Cancels the order and updates its status to cancelled.
    /// </summary>
    /// <remarks>After cancellation, a domain event is raised to notify other components of the change. Only
    /// orders that are in the Created or AwaitingPayment status can be cancelled; attempting to cancel orders in other
    /// states will result in an exception.</remarks>
    /// <param name="reason">The reason for cancelling the order. This value may be null or empty if no specific reason is provided.</param>
    /// <exception cref="InvalidOperationException">Thrown if the order status is not Created or AwaitingPayment.</exception>
    public void Cancel(string? reason)
    {
        if (Status != OrderStatus.Created &&
            Status != OrderStatus.AwaitingPayment)
            throw new InvalidOperationException("Only created or awaiting payment orders can be cancelled.");

        if (Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cancelled orders cannot be cancelled again.");

        var dateTimeNow = DateTime.UtcNow;
        Status = OrderStatus.Cancelled;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new OrderCancelledDomainEvent(
            Id, reason, dateTimeNow));
    }
}