namespace ProductCatalog.Domain.Shared.ValueObjects;

public record Money
{
    public Money(decimal amount, Currency currency)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (currency == Currency.None) throw new ArgumentException("Currency is required.", nameof(currency));

        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; private set; }
    public Currency Currency { get; private set; }

    public static Money operator +(Money first, Money second)
    {
        if (first.Currency != second.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies.");

        return new Money(first.Amount + second.Amount, first.Currency);
    }

    public static Money Zero(Currency currency) => new(0m, currency);
}