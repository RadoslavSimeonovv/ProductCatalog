using ProductCatalog.Application.Abstractions.Messaging;

namespace ProductCatalog.Application.Catalog.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string Description) : ICommand<Guid>;
