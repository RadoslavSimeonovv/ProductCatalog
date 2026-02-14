using ProductCatalog.Application.Messaging;

namespace ProductCatalog.Application.Catalog.UpdateCategoryCommand;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Description) : ICommand;