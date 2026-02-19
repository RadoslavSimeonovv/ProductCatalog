using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Shared.Errors;

namespace ProductCatalog.Domain.Shared.ValueObjects;

public sealed record CustomerId
{
    public string Value { get; }

    private CustomerId(string value) => Value = value;

    public static Result<CustomerId> Create(string value) 
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<CustomerId>(CustomerErrors.InvalidCustomerId);

        var trimmed = value.Trim();

        if (trimmed.Length is < 1 or > 100)
            return Result.Failure<CustomerId>(CustomerErrors.InvalidCustomerId);

        return Result.Success(new CustomerId(trimmed));
    }

    public static CustomerId FromDb(string value) => new CustomerId(value);
}