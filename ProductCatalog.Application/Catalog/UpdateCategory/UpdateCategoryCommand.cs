using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Catalog.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Description) : ICommand;