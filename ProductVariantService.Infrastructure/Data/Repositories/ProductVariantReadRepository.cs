using Dapper;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Domain.Models;

namespace ProductVariantService.Infrastructure.Data.Repositories;

internal sealed class ProductVariantReadRepository(IDbConnectionFactory connectionFactory) : IProductVariantReadRepository
{
    public async Task<IEnumerable<ProductVariantReadModel>> GetByProductIdAsync(
        Guid productId, 
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
                ProductCardId = productId
            },
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<ProductVariantReadModel>(command);
    }
}
