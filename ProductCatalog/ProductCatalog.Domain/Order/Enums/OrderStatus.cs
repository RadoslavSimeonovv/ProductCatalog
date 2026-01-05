namespace ProductCatalog.Domain.Order.Enums;

public enum OrderStatus
{
    Created,
    AwaitingPayment,
    Paid,
    Cancelled
}