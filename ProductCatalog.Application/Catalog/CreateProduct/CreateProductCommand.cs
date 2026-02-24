using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Application.Catalog.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    Money Price,
    Guid CategoryId,
    string Sku) : ICommand<Guid>;