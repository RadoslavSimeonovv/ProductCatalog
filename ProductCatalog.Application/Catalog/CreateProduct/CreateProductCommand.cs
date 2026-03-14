using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Domain.Shared.ValueObjects;

namespace ProductCatalog.Application.Catalog.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    string Sku,
    Guid CategoryId,
    decimal PriceAmount,
    string Currency) : ICommand<Guid>;