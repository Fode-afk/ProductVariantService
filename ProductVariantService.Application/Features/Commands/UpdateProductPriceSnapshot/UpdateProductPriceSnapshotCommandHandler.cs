using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Domain.ValueObjects;
using migApp.Shared.Results;
using ProductVariantService.Application.Caching;
using ProductVariantService.Application.Interfaces.Data;
using ZiggyCreatures.Caching.Fusion;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.UpdateProductPriceSnapshot;

public sealed class UpdateProductPriceSnapshotCommandHandler(
    IAppDbContext context,
    IFusionCache cache,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductPriceSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductPriceSnapshotCommand request, CancellationToken cancellationToken)
    {
        //var vendorResult = await vendorValidator.ValidateForModificationAsync(
        //    request.VendorId,
        //    cancellationToken);
        //if (vendorResult.IsFailure)
        //    return vendorResult;
        //
        //var productReadModel = await context.ProductReadModels
        //    .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        //if (productReadModel == null)
        //    return Fail(ProductErrors.NotFound());
        //
        //var cardResult = await productCardValidator.ValidateForModification(
        //    productReadModel.ProductCardId,
        //    request.VendorId,
        //    cancellationToken);
        //if (cardResult.IsFailure)
        //    return cardResult;
        //
        //var priceResult = Money.Create(request.Price, Currency.USD);
        //if (priceResult.IsFailure)
        //    return priceResult;
        //
        //Money? oldPrice = null;
        //if (request.OldPrice != null)
        //{
        //    var oldPriceResult = Money.Create(request.OldPrice.Value, Currency.USD);
        //    if (oldPriceResult.IsFailure)
        //        return oldPriceResult;
        //
        //    oldPrice = oldPriceResult.Value;
        //}
        //
        //if (oldPrice is not null && oldPrice < priceResult.Value)
        //    return Fail(ProductErrors.InvalidOldPrice());
        //
        //var now = timeProvider.GetUtcNow();
        //
        //productReadModel.PriceAmount = priceResult.Value.Amount;
        //productReadModel.OldPriceAmount = priceResult.Value.Amount;
        //productReadModel.PriceUpdatedAt = now;
        //productReadModel.UpdatedAt = now;
        //
        //await context.SaveChangesAsync(cancellationToken);
        //
        //await cache.RemoveByTagAsync(
        //    CacheTags.ProductsByCardId(productReadModel.ProductCardId),
        //    token: cancellationToken);

        return Ok();
    }
}
