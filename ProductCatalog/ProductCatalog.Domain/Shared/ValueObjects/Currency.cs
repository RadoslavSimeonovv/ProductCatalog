namespace ProductCatalog.Domain.Shared.ValueObjects;

public record Currency
{
    internal static readonly Currency None = new("");
    public static readonly Currency Usd = new("USD");
    public static readonly Currency Eur = new("EUR");

    public Currency(string code) => Code = code;
    public string Code { get; init; }

    public static Currency FromCode(string code) =>
        All.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase)) ??
        throw new ArgumentException($"Unsupported currency code: {code}", nameof(code));
    public static IReadOnlyCollection<Currency> All => [Usd, Eur];
}