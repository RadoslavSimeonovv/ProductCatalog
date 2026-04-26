using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Shared.Errors;

public static class CurrencyErrors
{
    public static Error Unsupported(string code) =>
        new("Currency.Unsupported", $"'{code}' is not a supported currency code.", ErrorType.Validation);
}
