using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Order.Errors;

public static class OrderErrors
{
    // Generic
    public static readonly Error NotFound =
        new("Order.NotFound", "Order was not found.", ErrorType.NotFound);

    public static readonly Error InvalidState =
        new("Order.InvalidState", "Order is in an invalid state for this operation.", ErrorType.Validation);

    public static readonly Error InvalidOrderId = 
        new("Order.InvalidOrderId", "Order ID must be a non-empty GUID.", ErrorType.Validation);

    // Creation / items
    public static readonly Error OrderItemsCannotBeNull =
        new("Order.OrderItemsCannotBeNull", "Order items cannot be null.", ErrorType.Validation);

    public static readonly Error EmptyOrder =
        new("Order.EmptyOrder", "Order must contain at least one item.", ErrorType.Validation);

    public static readonly Error CurrencyMismatch =
        new("Order.CurrencyMismatch", "All order items must have the same currency.", ErrorType.Validation);

    public static readonly Error CustomerEmailRequired =
        new("Order.CustomerEmailRequired", "Customer email is required.", ErrorType.Validation);

    public static readonly Error OrderItemOrderIdMismatch =
        new("Order.OrderItemOrderIdMismatch", "All order items must reference the same order ID.", ErrorType.Validation);

    // Submit / pay / cancel transitions
    public static readonly Error NotCreated =
        new("Order.NotCreated", "Only created orders can be submitted for payment.", ErrorType.Validation);

    public static readonly Error NotAwaitingPayment =
        new("Order.NotAwaitingPayment", "Only orders awaiting payment can be marked as paid.", ErrorType.Validation);

    public static readonly Error AlreadyPaid =
        new("Order.AlreadyPaid", "Order is already paid.", ErrorType.Validation);

    public static readonly Error AlreadyCancelled =
        new("Order.AlreadyCancelled", "Order is already cancelled.", ErrorType.Validation);

    public static readonly Error CannotCancelPaidOrder =
        new("Order.CannotCancelPaidOrder", "Paid orders cannot be cancelled.", ErrorType.Validation);

    public static readonly Error CannotSubmitCancelledOrder =
        new("Order.CannotSubmitCancelledOrder", "Cancelled orders cannot be submitted for payment.", ErrorType.Validation);

    public static readonly Error CannotPayCancelledOrder =
        new("Order.CannotPayCancelledOrder", "Cancelled orders cannot be marked as paid.", ErrorType.Validation);

    // Email
    public static readonly Error InvalidCustomerEmail =
        new("Order.InvalidCustomerEmail", "Customer email is invalid.", ErrorType.Validation);


    public static readonly Error ConcurrencyConflict =
        new("Order.ConcurrencyConflict", "Concurrency conflict occurred", ErrorType.Conflict);
}
