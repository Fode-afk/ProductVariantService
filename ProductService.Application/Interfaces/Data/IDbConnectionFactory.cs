using System.Data;

namespace ProductService.Application.Interfaces.Data;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
