using ProductService.Domain.Models;

namespace ProductService.Application.Interfaces.Data;

public interface IProductReadRepository
{
    Task<IEnumerable<ProductReadModel>> GetByCardIdAsync(
        Guid productCardId,
        CancellationToken cancellationToken = default);
}
