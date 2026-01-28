using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Catalog.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound =
       new("Product.NotFound", "Product was not found.");

    public static readonly Error InvalidState =
        new("Product.InvalidState", "Product is in an invalid state for this operation.");

    // Status transitions
    public static readonly Error AlreadyActive =
        new("Product.AlreadyActive", "Product is already active.");

    public static readonly Error StatusUnchanged =
        new("Product.StatusUnchanged", "New status is the same as the current one.");

    public static readonly Error NotActive =
        new("Product.NotActive", "Product must be active to perform this operation.");

    public static readonly Error InvalidStatus =
        new("Product.Invalid", "Product status is invalid.");

    public static readonly Error AlreadyInactive =
        new("Product.AlreadyInactive", "Product is already inactive.");

    public static readonly Error AlreadyDiscontinued =
        new("Product.AlreadyDiscontinued", "Product is already discontinued.");

    public static readonly Error DiscontinuedCannotBeActivated =
        new("Product.DiscontinuedCannotBeActivated", "Discontinued products cannot be activated.");

    public static readonly Error DiscontinuedCannotBeModified =
        new("Product.DiscontinuedCannotBeModified", "Discontinued products cannot be modified.");

    // Category changes
    public static readonly Error CategoryUnchanged =
        new("Product.CategoryUnchanged", "New category is the same as the current category.");

    public static readonly Error InvalidCategoryId =
        new("Product.InvalidCategoryId", "CategoryId cannot be empty.");

    // Price changes
    public static readonly Error PriceUnchanged =
        new("Product.PriceUnchanged", "New price is the same as the current price.");

    public static readonly Error InvalidPrice =
        new("Product.InvalidPrice", "Price must be valid and greater than or equal to zero.");

    public static readonly Error CurrencyChangeNotAllowed =
        new("Product.CurrencyChangeNotAllowed", "Changing product currency is not allowed.");

    // Feature operations
    public static readonly Error InvalidFeatureId =
        new("Product.InvalidFeatureId", "FeatureId cannot be empty.");

    public static readonly Error FeatureNotFound =
        new("Product.FeatureNotFound", "Feature was not found for this product.");

    public static readonly Error DuplicateFeatureId =
        new("Product.DuplicateFeatureId", "FeatureId must be unique within the product.");

    public static readonly Error FeatureAlreadyExists =
        new("Product.FeatureAlreadyExists", "A feature with the same name already exists for this product.");

    public static readonly Error InvalidFeatureName =
        new("Product.InvalidFeatureName", "Feature name cannot be empty.");

    public static readonly Error InvalidFeatureValue =
        new("Product.InvalidFeatureValue", "Feature value cannot be empty.");

    public static readonly Error FeatureValueUnchanged =
        new("Product.FeatureValueUnchanged", "New feature value is the same as the current value.");
}