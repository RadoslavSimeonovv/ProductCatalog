using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Domain.Catalog.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound =
       new("Product.NotFound", "Product was not found.", ErrorType.NotFound);

    public static readonly Error InvalidState =
        new("Product.InvalidState", "Product is in an invalid state for this operation.", ErrorType.Validation);

    public static readonly Error InvalidName =
        new("Product.InvalidName", "Product name is not valid.", ErrorType.Validation);

    // Status transitions
    public static readonly Error AlreadyActive =
        new("Product.AlreadyActive", "Product is already active.", ErrorType.Validation);

    public static readonly Error StatusUnchanged =
        new("Product.StatusUnchanged", "New status is the same as the current one.", ErrorType.Validation);

    public static readonly Error NotActive =
        new("Product.NotActive", "Product must be active to perform this operation.", ErrorType.Validation);

    public static readonly Error InvalidStatus =
        new("Product.Invalid", "Product status is invalid.", ErrorType.Validation);

    public static readonly Error AlreadyInactive =
        new("Product.AlreadyInactive", "Product is already inactive.", ErrorType.Validation);

    public static readonly Error AlreadyDiscontinued =
        new("Product.AlreadyDiscontinued", "Product is already discontinued.", ErrorType.Validation);

    public static readonly Error DiscontinuedCannotBeActivated =
        new("Product.DiscontinuedCannotBeActivated", "Discontinued products cannot be activated.", ErrorType.Validation);

    public static readonly Error DiscontinuedCannotBeModified =
        new("Product.DiscontinuedCannotBeModified", "Discontinued products cannot be modified.", ErrorType.Validation);

    // Category errors
    public static readonly Error InvalidCategoryName =
        new("Product.InvalidCategoryName", "Category name is invalid.", ErrorType.Validation);

    public static readonly Error CategoryUnchanged =
        new("Product.CategoryUnchanged", "New category is the same as the current category.", ErrorType.Validation);

    public static readonly Error InvalidCategoryId =
        new("Product.InvalidCategoryId", "CategoryId cannot be empty.", ErrorType.Validation);

    public static readonly Error CategoryNotFound =
        new("Product.CategoryNotFound", "Category was not found.", ErrorType.NotFound);

    // Price errors
    public static readonly Error PriceUnchanged =
        new("Product.PriceUnchanged", "New price is the same as the current price.", ErrorType.Validation);

    public static readonly Error InvalidPrice =
        new("Product.InvalidPrice", "Price must be valid and greater than or equal to zero.", ErrorType.Validation);

    public static readonly Error CurrencyChangeNotAllowed =
        new("Product.CurrencyChangeNotAllowed", "Changing product currency is not allowed.", ErrorType.Validation);

    // Feature operations
    public static readonly Error InvalidFeatureId =
        new("Product.InvalidFeatureId", "FeatureId cannot be empty.", ErrorType.Validation);

    public static readonly Error FeatureNotFound =
        new("Product.FeatureNotFound", "Feature was not found.", ErrorType.NotFound);

    public static readonly Error DuplicateFeatureId =
        new("Product.DuplicateFeatureId", "FeatureId must be unique within the product.", ErrorType.Conflict);

    public static readonly Error FeatureAlreadyExists =
        new("Product.FeatureAlreadyExists", "A feature with the same name already exists for this product.", ErrorType.Conflict);

    public static readonly Error InvalidFeatureName =
        new("Product.InvalidFeatureName", "Feature name cannot be empty.", ErrorType.Validation);

    public static readonly Error InvalidFeatureValue =
        new("Product.InvalidFeatureValue", "Feature value cannot be empty.", ErrorType.Validation);

    public static readonly Error FeatureValueUnchanged =
        new("Product.FeatureValueUnchanged", "New feature value is the same as the current value.", ErrorType.Validation);

    // Sku errors
    public static readonly Error InvalidSku =
        new("Product.InvalidSku", "SKU value is not valid.", ErrorType.Validation);

    public static readonly Error ConcurrencyConflict =
        new("Product.ConcurrencyConflict", "Concurrency conflict occurred", ErrorType.Conflict);
}