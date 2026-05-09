using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Caching;
using ProductVariantService.Application.Interfaces.Data;
using ZiggyCreatures.Caching.Fusion;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.UpdateProductStockSnapshot;

public sealed class UpdateProductStockSnapshotCommandHandler(
    IAppDbContext context,
    IFusionCache cache,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductStockSnapshotCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductStockSnapshotCommand request, CancellationToken cancellationToken)
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
        //if (request.AvailableQuantity < 0)
        //    return Fail(ProductErrors.InvalidAvailableQuantity());
        //
        //var now = timeProvider.GetUtcNow();
        //
        //productReadModel.Status = request.Status;
        //productReadModel.AvailableQuantity = request.AvailableQuantity;
        //productReadModel.StockUpdatedAt = now;
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
