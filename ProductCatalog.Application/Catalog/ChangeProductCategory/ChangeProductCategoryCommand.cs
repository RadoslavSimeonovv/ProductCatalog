using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Catalog.ChangeProductCategory;

public sealed record ChangeProductCategoryCommand(
    Guid ProductId,
    Guid NewCategoryId) : ICommand;