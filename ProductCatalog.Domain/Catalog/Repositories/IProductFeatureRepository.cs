using ProductCatalog.Domain.Catalog.Entities;

namespace ProductCatalog.Domain.Catalog.Repositories;

public interface IProductFeatureRepository
{
    Task<ProductFeature?> GetByIdAsync(Guid featureId, CancellationToken cancellationToken = default);
}
