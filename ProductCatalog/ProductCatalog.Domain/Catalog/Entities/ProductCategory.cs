using ProductCatalog.Domain.Abstractions;
using ProductCatalog.Domain.Catalog.Errors;

namespace ProductCatalog.Domain.Catalog.Entities;

/// <summary>
/// </summary>
public sealed class ProductCategory : Entity
{
    public ProductCategory(Guid id,
        string name,
        string description,
        DateTime createdAt)
        : base(id)
    {
        Name = name;
        Description = description;
        CreatedAt = createdAt;
    }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static Result<ProductCategory> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<ProductCategory>(ProductErrors.InvalidCategoryName);
        }

        var trimmedName = name.Trim();
        description = string.IsNullOrWhiteSpace(description) ? string.Empty : description.Trim();

        var category = new ProductCategory(
            Guid.NewGuid(),
            trimmedName, 
            description, 
            DateTime.UtcNow);

        return Result.Success(category);
    }

    public Result UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(ProductErrors.InvalidCategoryName);
        }

        var trimmedName = name.Trim();
        var trimmedDescription = string.IsNullOrWhiteSpace(description)
            ? string.Empty
            : description.Trim();

        if (Name == trimmedName && Description == trimmedDescription)
            return Result.Success();

        Name = trimmedName;
        Description = trimmedDescription;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}