using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;

namespace ProductCatalog.Domain.Catalog.ValueObjects;

public record Sku
{
    private Sku(string value)
    {
        value = Value!;
    }

    public string Value { get; private set; }

    public static Result<Sku> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<Sku>(ProductErrors.InvalidSku);
        }

        return Result.Success(new Sku(value));
    }
}