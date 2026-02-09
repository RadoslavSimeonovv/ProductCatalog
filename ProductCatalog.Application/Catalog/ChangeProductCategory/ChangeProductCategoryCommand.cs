using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Catalog.ChangeProductCategory;

public sealed record ChangeProductCategoryCommand(
    Guid ProductId,
    Guid NewCategoryId) : ICommand;