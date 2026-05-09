using MediatR;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using ProductVariantService.Application.Caching;
using ProductVariantService.Application.Dtos;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Application.Interfaces.Services;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Models;
using System.Text.Json;
using ZiggyCreatures.Caching.Fusion;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Queries.GetProductsByCardId;

public sealed class GetProductsByCardIdQueryHandler(
    IProductVariantReadRepository productReadRepository,
    IMoneyConverter moneyConverter,
    IFusionCache cache) : IRequestHandler<GetProductsByCardIdQuery, IResult<IEnumerable<ProductDto>>>
{
    public async Task<IResult<IEnumerable<ProductDto>>> Handle(GetProductsByCardIdQuery request, CancellationToken cancellationToken)
    {
        var currencyResult = Currency.Create(request.Currency);
        if (currencyResult.IsFailure)
            return Fail<IEnumerable<ProductDto>>(currencyResult.Error);

        var currency = currencyResult.Value;

        var productDtos = await cache.GetOrSetAsync<IEnumerable<ProductDto>>(
            CacheKeys.ProductVariantsByProductId(request.ProductCardId, currency.Code),
            async (entry, ct) => await GetProductDtos(
                request.ProductCardId,
                currency,
                ct),
            tags: [CacheTags.ProductVariantsByProductId(request.ProductCardId)],
            token: cancellationToken);

        return productDtos != null && productDtos.Any() ?
            Ok(productDtos) :
            Fail<IEnumerable<ProductDto>>(ProductVariantErrors.NotFound());
    }

    private async Task<IEnumerable<ProductDto>> GetProductDtos(
        Guid productCardId,
        Currency currency,
        CancellationToken cancellationToken = default)
    {
        var products = await productReadRepository.GetByProductIdAsync(productCardId, cancellationToken);   

        var dtoTasks = products.Select(
            product => MapToProductDtoAsync(
                product,
                currency,
                cancellationToken));

        var dtos = await Task.WhenAll(dtoTasks);

        return dtos
            .Where(dto => dto is not null)!
            .Cast<ProductDto>();
    }

    private async Task<ProductDto?> MapToProductDtoAsync(
        ProductVariantReadModel product,
        Currency targetCurrency,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        long priceAmountMinor = 0;
        long oldPriceAmountMinor = 0;

        if (product.PriceAmount is not null)
        {
            var convertedPriceResult = await ConvertUsdToTargetMinorAsync(
                product.PriceAmount.Value,
                targetCurrency,
                cancellationToken);

            if (convertedPriceResult.IsFailure)
                return null;

            priceAmountMinor = convertedPriceResult.Value;

            if (product.OldPriceAmount is not null)
            {
                var convertedOldPriceResult = await ConvertUsdToTargetMinorAsync(
                    product.OldPriceAmount.Value,
                    targetCurrency,
                    cancellationToken);

                if (convertedOldPriceResult.IsFailure)
                    return null;

                oldPriceAmountMinor = convertedOldPriceResult.Value;
            }
        }

        return new ProductDto(
            product.Id.ToString(),
            product.SKU,
            product.Name,
            product.Barcode,
            new DimensionsDto(
                product.Length,
                product.Width,
                product.Height,
                product.DimensionUnit),
            new WeightDto(
                product.Weight,
                product.WeightUnit),
            priceAmountMinor,
            oldPriceAmountMinor,
            product.Status,
            product.AvailableQuantity,
            JsonSerializer.Deserialize<Dictionary<string, string>>(product.AttributesJson)!,
            JsonSerializer.Deserialize<List<string>>(product.TagsJson)!,
            JsonSerializer.Deserialize<List<ProductImageDto>>(product.ImagesJson)!);
    }

    private async Task<IResult<long>> ConvertUsdToTargetMinorAsync(
        decimal usdAmount,
        Currency targetCurrency,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var usdMoneyResult = Money.Create(usdAmount, Currency.USD);
        if (usdMoneyResult.IsFailure)
            return Fail<long>(usdMoneyResult.Error);

        var money = usdMoneyResult.Value;

        if (targetCurrency != Currency.USD)
        {
            var converted = await moneyConverter.ConvertAsync(
                money,
                targetCurrency,
                cancellationToken);

            if (converted.IsFailure)
                return Fail<long>(converted.Error);

            money = converted.Value;
        }

        var minorResult = Money.ToMinor(money.Amount, targetCurrency);

        return minorResult.IsFailure
            ? Fail<long>(minorResult.Error)
            : Ok(minorResult.Value);
    }
}