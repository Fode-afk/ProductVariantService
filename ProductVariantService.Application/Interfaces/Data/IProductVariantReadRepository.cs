using ProductVariantService.Domain.Models;

namespace ProductVariantService.Application.Interfaces.Data;

public interface IProductVariantReadRepository
{
    Task<IEnumerable<ProductVariantReadModel>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}
