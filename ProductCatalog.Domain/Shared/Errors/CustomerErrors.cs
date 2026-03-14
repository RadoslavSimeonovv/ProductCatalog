using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Shared.Errors;

public static class CustomerErrors
{
    public static readonly Error InvalidCustomerId =
        new("Customer.InvalidCustomerId", "Customer Id is invalid.", ErrorType.Validation);
}
