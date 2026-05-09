using System.Data;

namespace ProductVariantService.Application.Interfaces.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
