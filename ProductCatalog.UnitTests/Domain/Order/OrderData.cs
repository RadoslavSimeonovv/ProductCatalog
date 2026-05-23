using OrderEntity = ProductCatalog.Domain.Order.Entities.Order;
using ProductCatalog.Domain.Order.Entities;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.UnitTests.Domain.Order;

internal static class OrderData
{
    public static readonly Guid OrderId = new Guid("b1b2b3b4-0000-0000-0000-000000000001");
    public static readonly Guid ProductId = new Guid("c1c2c3c4-0000-0000-0000-000000000001");
    public static readonly CustomerId CustomerId = CustomerId.Create("user-123").Value;
    public static readonly string CustomerEmail = "customer@test.com";
    public static readonly Money UnitPrice = new Money(10.00m, new Currency("USD"));

    public static OrderItem CreateItem(Guid orderId) =>
        new OrderItem(Guid.NewGuid(), orderId, ProductId, 2, UnitPrice);

    public static OrderEntity CreateOrder()
    {
        var items = new List<OrderItem> { CreateItem(OrderId) };
        return OrderEntity.Create(OrderId, CustomerId, CustomerEmail, items).Value;
    }

    public static OrderEntity CreateSubmittedOrder()
    {
        var order = CreateOrder();
        order.SubmitForPayment();
        return order;
    }

    public static OrderEntity CreatePaidOrder()
    {
        var order = CreateSubmittedOrder();
        order.MarkAsPaid();
        return order;
    }

    public static OrderEntity CreateCancelledOrder()
    {
        var order = CreateOrder();
        order.Cancel(null);
        return order;
    }
}
