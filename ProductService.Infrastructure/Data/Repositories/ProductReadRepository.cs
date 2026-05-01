using Dapper;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Models;

namespace ProductService.Infrastructure.Data.Repositories;

internal sealed class ProductReadRepository(IDbConnectionFactory connectionFactory) : IProductReadRepository
{
    public async Task<IEnumerable<ProductReadModel>> GetByCardIdAsync(
        Guid productCardId, 
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                [Id],
                [ProductCardId],
                [SKU],
                [Name],
                [NameNormalized],
                [Barcode],

                [Length],
                [Width],
                [Height],
                [DimensionUnit],

                [Weight],
                [WeightUnit],

                [MainImage],
                [AttributesJson],
                [TagsJson],
                [TagsFlat],
                [ImagesJson],

                [IsDefault],

                [PriceAmount],
                [OldPriceAmount],
                [PriceUpdatedAt],

                [Status],
                [AvailableQuantity],
                [StockUpdatedAt],

                [CreatedAt],
                [UpdatedAt]
            FROM [products].[ProductReadModels]
            WHERE [ProductCardId] = @ProductCardId
            ORDER BY [IsDefault] DESC, [CreatedAt] ASC
            """;

        using var connection = await connectionFactory.CreateConnectionAsync(cancellationToken);

        var command = new CommandDefinition(
            sql,
            new
            {
                ProductCardId = productCardId
            },
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<ProductReadModel>(command);
    }
}
