using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Order.Enums;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Events;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Domain.Order.Entities;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _items;
    private Order(Guid id,
        CustomerId customerId,
        string customerEmail,
        Money totalAmount,
        List<OrderItem> items,
        DateTime createdAt) : base(id)
    {
        Id = id;
        CustomerId = customerId;
        CustomerEmail = customerEmail;
        TotalAmount = totalAmount;
        Status = OrderStatus.Created;
        _items = [.. items];
        CreatedAt = createdAt;
    }
    public CustomerId CustomerId { get; private set; }
    public string CustomerEmail { get; private set; }
    public Money TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Static factory method to create a new order instance. Validates the input parameters and constructs the order
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="customerId"></param>
    /// <param name="customerEmail"></param>
    /// <param name="items"></param>
    /// <returns></returns>
    public static Result<Order> Create(Guid orderId, CustomerId customerId, string customerEmail, List<OrderItem> items)
    {
        if (orderId == Guid.Empty)
            return Result.Failure<Order>(OrderErrors.InvalidOrderId);

        if (string.IsNullOrWhiteSpace(customerEmail))
            return Result.Failure<Order>(OrderErrors.InvalidCustomerEmail);

        if (items is null)
            return Result.Failure<Order>(OrderErrors.OrderItemsCannotBeNull);

        if (items.Count == 0)
            return Result.Failure<Order>(OrderErrors.EmptyOrder);

        var customerEmailTrimmed = customerEmail.Trim();
        var orderItems = new List<OrderItem>(items);

        if (orderItems.Any(i => i.OrderId != orderId))
            return Result.Failure<Order>(OrderErrors.OrderItemOrderIdMismatch);

        var orderCurrency = orderItems[0].LineTotal.Currency;
        var orderTotal = Money.Zero(orderCurrency);

        foreach (var item in orderItems)
        {
            if (item.LineTotal.Currency != orderCurrency)
                return Result.Failure<Order>(OrderErrors.CurrencyMismatch);

            orderTotal += item.LineTotal;
        }

        var now = DateTime.UtcNow;

        var order = new Order(
            id: orderId,
            customerId: customerId,
            customerEmail: customerEmailTrimmed,
            totalAmount: orderTotal,
            items: orderItems,
            createdAt: now);

        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id, order.CustomerEmail, order.TotalAmount, now));

        return Result.Success(order);
    }

    /// <summary>
    /// Submits the order for payment processing if it is in the created state and contains at least one item.
    /// </summary>
    /// <remarks>If the submission is successful, the order status is updated to awaiting payment and a domain
    /// event is raised. This method does not initiate payment itself; it prepares the order for the payment
    /// process.</remarks>
    /// <returns>A <see cref="Result"/> indicating whether the order was successfully submitted for payment. Returns a failure
    /// result if the order is not in the created state or contains no items.</returns>
    public Result SubmitForPayment()
    {
        if (Status != OrderStatus.Created)
            return Result.Failure(OrderErrors.NotCreated);

        if (_items.Count == 0)
            return Result.Failure(OrderErrors.EmptyOrder);

        var dateTimeNow = DateTime.UtcNow;
        Status = OrderStatus.AwaitingPayment;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new OrderSubmittedForPaymentDomainEvent(
            Id, TotalAmount, dateTimeNow));

        return Result.Success();
    }

    /// <summary>
    /// Marks the order as paid if it is currently awaiting payment.
    /// </summary>
    /// <remarks>This method updates the order status to paid and raises an order paid domain event. If the
    /// order is not awaiting payment, the status is not changed and a failure result is returned.</remarks>
    /// <returns>A <see cref="Result"/> indicating whether the operation succeeded. Returns a failure result if the order is not
    /// in the awaiting payment status.</returns>
    public Result MarkAsPaid()
    {
        if (Status != OrderStatus.AwaitingPayment)
            return Result.Failure(OrderErrors.NotAwaitingPayment);

        var dateTimeNow = DateTime.UtcNow;
        Status = OrderStatus.Paid;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new OrderPaidDomainEvent(
            Id, TotalAmount, dateTimeNow));

        return Result.Success();
    }

    /// <summary>
    /// Marks the order as cancelled if it is in a cancellable state (created or awaiting payment).
    /// </summary>
    /// <param name="reason"></param>
    /// <returns> A <see cref="Result"/> indicating where the operation succeeded. Returns a failure result if the order is not
    /// in a created or awaiting payment state.</returns>
    public Result Cancel(string? reason)
    {
        if (Status == OrderStatus.Cancelled)
            return Result.Failure(OrderErrors.AlreadyCancelled);

        if (Status == OrderStatus.Paid)
            return Result.Failure(OrderErrors.CannotCancelPaidOrder);

        if (Status != OrderStatus.Created &&
            Status != OrderStatus.AwaitingPayment)
            return Result.Failure(OrderErrors.InvalidState);

        reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();

        var dateTimeNow = DateTime.UtcNow;
        Status = OrderStatus.Cancelled;
        UpdatedAt = dateTimeNow;

        RaiseDomainEvent(new OrderCancelledDomainEvent(
            Id, reason, dateTimeNow));

        return Result.Success();
    }
}