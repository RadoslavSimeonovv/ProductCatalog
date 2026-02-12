using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;
using System.Text.RegularExpressions;

namespace ProductCatalog.Domain.Catalog.ValueObjects;

public record Sku
{
    private const int MaxLength = 32;
    private static readonly Regex Format = new("^[A-Za-z0-9_-]+$", RegexOptions.Compiled);

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

        var trimmed = value.Trim();

        if (trimmed.Length > MaxLength)
            return Result.Failure<Sku>(ProductErrors.InvalidSku);

        if (!Format.IsMatch(trimmed))
            return Result.Failure<Sku>(ProductErrors.InvalidSku);

        return Result.Success(new Sku(trimmed));
    }
}