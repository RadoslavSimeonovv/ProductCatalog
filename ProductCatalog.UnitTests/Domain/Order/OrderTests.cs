using OrderEntity = ProductCatalog.Domain.Order.Entities.Order;
using FluentAssertions;
using ProductCatalog.Domain.Order.Entities;
using ProductCatalog.Domain.Order.Enums;
using ProductCatalog.Domain.Order.Errors;
using ProductCatalog.Domain.Order.Events;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.UnitTests.Domain.Order;

public class OrderTests
{
    #region Create

    [Fact]
    public void Create_Should_ReturnFailure_WhenOrderIdIsEmpty()
    {
        // Arrange — items need a valid orderId; Order.Create checks orderId == Guid.Empty before item validation
        var items = new List<OrderItem> { OrderData.CreateItem(Guid.NewGuid()) };

        // Act
        var result = OrderEntity.Create(Guid.Empty, OrderData.CustomerId, OrderData.CustomerEmail, items);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidOrderId);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenCustomerEmailIsEmpty()
    {
        // Arrange
        var items = new List<OrderItem> { OrderData.CreateItem(OrderData.OrderId) };

        // Act
        var result = OrderEntity.Create(OrderData.OrderId, OrderData.CustomerId, "", items);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.InvalidCustomerEmail);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenItemsIsNull()
    {
        // Act
        var result = OrderEntity.Create(OrderData.OrderId, OrderData.CustomerId, OrderData.CustomerEmail, null!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.OrderItemsCannotBeNull);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenItemsIsEmpty()
    {
        // Act
        var result = OrderEntity.Create(OrderData.OrderId, OrderData.CustomerId, OrderData.CustomerEmail, []);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.EmptyOrder);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenItemOrderIdDoesNotMatchOrderId()
    {
        // Arrange — item belongs to a different order
        var differentOrderId = Guid.NewGuid();
        var items = new List<OrderItem> { OrderData.CreateItem(differentOrderId) };

        // Act
        var result = OrderEntity.Create(OrderData.OrderId, OrderData.CustomerId, OrderData.CustomerEmail, items);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.OrderItemOrderIdMismatch);
    }

    [Fact]
    public void Create_Should_ReturnFailure_WhenItemsHaveMixedCurrencies()
    {
        // Arrange
        var itemUsd = OrderData.CreateItem(OrderData.OrderId);
        var itemEur = new OrderItem(Guid.NewGuid(), OrderData.OrderId, OrderData.ProductId, 1, new Money(5.00m, new Currency("EUR")));
        var items = new List<OrderItem> { itemUsd, itemEur };

        // Act
        var result = OrderEntity.Create(OrderData.OrderId, OrderData.CustomerId, OrderData.CustomerEmail, items);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.CurrencyMismatch);
    }

    [Fact]
    public void Create_Should_ReturnSuccess_WhenAllParametersAreValid()
    {
        // Arrange
        var items = new List<OrderItem> { OrderData.CreateItem(OrderData.OrderId) };

        // Act
        var result = OrderEntity.Create(OrderData.OrderId, OrderData.CustomerId, OrderData.CustomerEmail, items);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(OrderData.OrderId);
        result.Value.CustomerId.Should().Be(OrderData.CustomerId);
        result.Value.CustomerEmail.Should().Be(OrderData.CustomerEmail);
        result.Value.Status.Should().Be(OrderStatus.Created);
        result.Value.Items.Should().HaveCount(1);
    }

    [Fact]
    public void Create_Should_CalculateTotalAmount_FromItems()
    {
        // Arrange — 2 items at qty 2 × $10 each = $40 total
        var item1 = OrderData.CreateItem(OrderData.OrderId);
        var item2 = OrderData.CreateItem(OrderData.OrderId);
        var items = new List<OrderItem> { item1, item2 };

        // Act
        var result = OrderEntity.Create(OrderData.OrderId, OrderData.CustomerId, OrderData.CustomerEmail, items);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalAmount.Amount.Should().Be(item1.LineTotal.Amount + item2.LineTotal.Amount);
    }

    [Fact]
    public void Create_Should_RaiseDomainEvent_WhenOrderIsCreated()
    {
        // Arrange
        var items = new List<OrderItem> { OrderData.CreateItem(OrderData.OrderId) };

        // Act
        var result = OrderEntity.Create(OrderData.OrderId, OrderData.CustomerId, OrderData.CustomerEmail, items);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var evt = result.Value.GetDomainEvents().OfType<OrderCreatedDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.OrderId.Should().Be(OrderData.OrderId);
        evt.CustomerEmail.Should().Be(OrderData.CustomerEmail);
    }

    #endregion

    #region SubmitForPayment

    [Fact]
    public void SubmitForPayment_Should_ReturnSuccess_WhenOrderIsCreated()
    {
        // Arrange
        var order = OrderData.CreateOrder();

        // Act
        var result = order.SubmitForPayment();

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.AwaitingPayment);
    }

    [Fact]
    public void SubmitForPayment_Should_ReturnFailure_WhenOrderIsNotCreated()
    {
        // Arrange — already submitted
        var order = OrderData.CreateSubmittedOrder();

        // Act
        var result = order.SubmitForPayment();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.NotCreated);
    }

    [Fact]
    public void SubmitForPayment_Should_RaiseDomainEvent_WhenSubmitted()
    {
        // Arrange
        var order = OrderData.CreateOrder();

        // Act
        order.SubmitForPayment();

        // Assert
        var evt = order.GetDomainEvents().OfType<OrderSubmittedForPaymentDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.OrderId.Should().Be(order.Id);
        evt.Total.Should().Be(order.TotalAmount);
    }

    #endregion

    #region MarkAsPaid

    [Fact]
    public void MarkAsPaid_Should_ReturnSuccess_WhenOrderIsAwaitingPayment()
    {
        // Arrange
        var order = OrderData.CreateSubmittedOrder();

        // Act
        var result = order.MarkAsPaid();

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public void MarkAsPaid_Should_ReturnFailure_WhenOrderIsNotAwaitingPayment()
    {
        // Arrange — order is Created, not AwaitingPayment
        var order = OrderData.CreateOrder();

        // Act
        var result = order.MarkAsPaid();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.NotAwaitingPayment);
    }

    [Fact]
    public void MarkAsPaid_Should_RaiseDomainEvent_WhenOrderIsPaid()
    {
        // Arrange
        var order = OrderData.CreateSubmittedOrder();

        // Act
        order.MarkAsPaid();

        // Assert
        var evt = order.GetDomainEvents().OfType<OrderPaidDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.OrderId.Should().Be(order.Id);
        evt.Total.Should().Be(order.TotalAmount);
    }

    #endregion

    #region Cancel

    [Fact]
    public void Cancel_Should_ReturnSuccess_WhenOrderIsCreated()
    {
        // Arrange
        var order = OrderData.CreateOrder();

        // Act
        var result = order.Cancel("Changed my mind");

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_Should_ReturnSuccess_WhenOrderIsAwaitingPayment()
    {
        // Arrange
        var order = OrderData.CreateSubmittedOrder();

        // Act
        var result = order.Cancel("No longer needed");

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_Should_ReturnFailure_WhenOrderIsAlreadyCancelled()
    {
        // Arrange
        var order = OrderData.CreateCancelledOrder();

        // Act
        var result = order.Cancel(null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.AlreadyCancelled);
    }

    [Fact]
    public void Cancel_Should_ReturnFailure_WhenOrderIsPaid()
    {
        // Arrange
        var order = OrderData.CreatePaidOrder();

        // Act
        var result = order.Cancel(null);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrderErrors.CannotCancelPaidOrder);
    }

    [Fact]
    public void Cancel_Should_RaiseDomainEvent_WhenOrderIsCancelled()
    {
        // Arrange
        var order = OrderData.CreateOrder();
        var reason = "Out of stock";

        // Act
        order.Cancel(reason);

        // Assert
        var evt = order.GetDomainEvents().OfType<OrderCancelledDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.OrderId.Should().Be(order.Id);
        evt.CustomerEmail.Should().Be(OrderData.CustomerEmail);
        evt.Reason.Should().Be(reason);
    }

    [Fact]
    public void Cancel_Should_SetReasonToNull_WhenReasonIsWhitespace()
    {
        // Arrange
        var order = OrderData.CreateOrder();

        // Act
        order.Cancel("   ");

        // Assert
        var evt = order.GetDomainEvents().OfType<OrderCancelledDomainEvent>().SingleOrDefault();
        evt.Should().NotBeNull();
        evt!.Reason.Should().BeNull();
    }

    #endregion
}
