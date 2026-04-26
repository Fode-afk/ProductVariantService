using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductService.Application.Caching;
using ProductService.Application.Dtos;
using ProductService.Application.Interfaces.Data;
using ProductService.Domain.Errors;
using System.Text.Json;
using ZiggyCreatures.Caching.Fusion;
using static migApp.Shared.Results.ResultFactory;

namespace ProductService.Application.Features.Queries.GetProductsByCardId;

public sealed class GetProductsByCardIdQueryHandler(
    IAppDbContext context,
    IFusionCache cache) : IRequestHandler<GetProductsByCardIdQuery, IResult<IEnumerable<ProductDto>>>
{
    public async Task<IResult<IEnumerable<ProductDto>>> Handle(GetProductsByCardIdQuery request, CancellationToken cancellationToken)
    {
        var productDtos = await cache.GetOrSetAsync<IEnumerable<ProductDto>>(
            CacheKeys.ProductsByCardId(request.ProductCardId),
            async (entry, ct) =>
                await context.ProductReadModels                  
                    .Where(p => p.ProductCardId == request.ProductCardId)
                    .Select(p => new ProductDto(
                        p.Id.ToString(),
                        p.SKU,
                        p.Name,
                        p.Barcode,
                        new DimensionsDto(
                            p.Length,
                            p.Width,
                            p.Height,
                            p.DimensionUnit),
                        new WeightDto(
                            p.Weight,
                            p.WeightUnit),
                        JsonSerializer.Deserialize<Dictionary<string, string>>(p.AttributesJson)!,
                        JsonSerializer.Deserialize<List<string>>(p.TagsJson)!,
                        JsonSerializer.Deserialize<List<ProductImageDto>>(p.ImagesJson)!))
                    .ToListAsync(cancellationToken),
                token: cancellationToken);

        return productDtos != null && productDtos.Any() ?
            Ok(productDtos) :
            Fail<IEnumerable<ProductDto>>(ProductErrors.NotFound());
    }
}