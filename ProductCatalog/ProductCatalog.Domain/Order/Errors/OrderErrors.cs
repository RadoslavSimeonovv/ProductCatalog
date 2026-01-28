using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Order.Errors;

public static class OrderErrors
{
    // Generic
    public static readonly Error NotFound =
        new("Order.NotFound", "Order was not found.");

    public static readonly Error InvalidState =
        new("Order.InvalidState", "Order is in an invalid state for this operation.");

    // Creation / items
    public static readonly Error OrderItemsCannotBeNull =
        new("Order.OrderItemsCannotBeNull", "Order items cannot be null.");

    public static readonly Error EmptyOrder =
        new("Order.EmptyOrder", "Order must contain at least one item.");

    public static readonly Error CurrencyMismatch =
        new("Order.CurrencyMismatch", "All order items must have the same currency.");

    public static readonly Error CustomerEmailRequired =
        new("Order.CustomerEmailRequired", "Customer email is required.");

    // Submit / pay / cancel transitions
    public static readonly Error NotCreated =
        new("Order.NotCreated", "Only created orders can be submitted for payment.");

    public static readonly Error NotAwaitingPayment =
        new("Order.NotAwaitingPayment", "Only orders awaiting payment can be marked as paid.");

    public static readonly Error AlreadyPaid =
        new("Order.AlreadyPaid", "Order is already paid.");

    public static readonly Error AlreadyCancelled =
        new("Order.AlreadyCancelled", "Order is already cancelled.");

    public static readonly Error CannotCancelPaidOrder =
        new("Order.CannotCancelPaidOrder", "Paid orders cannot be cancelled.");

    public static readonly Error CannotSubmitCancelledOrder =
        new("Order.CannotSubmitCancelledOrder", "Cancelled orders cannot be submitted for payment.");

    public static readonly Error CannotPayCancelledOrder =
        new("Order.CannotPayCancelledOrder", "Cancelled orders cannot be marked as paid.");

    // Email
    public static readonly Error InvalidCustomerEmail =
        new("Order.InvalidCustomerEmail", "Customer email is invalid.");
}
