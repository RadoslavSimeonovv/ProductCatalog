using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Shared.Errors;

namespace ProductCatalog.Domain.Shared.ValueObjects;

public record Currency
{
    internal static readonly Currency None = new("");
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");

    public Currency(string code) => Code = code;
    public string Code { get; init; }

    public static Result<Currency> FromCode(string code)
    {
        var currency = All.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        return currency is not null
            ? Result.Success(currency)
            : Result.Failure<Currency>(CurrencyErrors.Unsupported(code));
    }
    public static IReadOnlyCollection<Currency> All => [Usd, Eur];
}