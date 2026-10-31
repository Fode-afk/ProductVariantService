using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Dtos;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Specifications.Common;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.RemoveProductVariantImage;

public sealed class RemoveProductVariantImageCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<RemoveProductVariantImageCommand, IResult<ProductVariantWriteAck>>
{
    public async Task<IResult<ProductVariantWriteAck>> Handle(RemoveProductVariantImageCommand request, CancellationToken cancellationToken)
    {
        var productVariant = await context.ProductVariants
            .Include(v => v.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductVariantId, cancellationToken);
        if (productVariant == null)
            return Fail<ProductVariantWriteAck>(ProductVariantErrors.NotFound());

        var productSnapshot = await context.ProductSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.ProductId == productVariant.ProductId,
                cancellationToken);
        if (productSnapshot is null)
            return Fail<ProductVariantWriteAck>(ProductSnapshotErrors.NotFound());

        var ownershipCtx = new ProductVendorOwnershipContext(
            request.VendorId,
            productSnapshot.VendorId);

        var ownershipResult = ProductBelongsToVendorSpec.Instance.IsSatisfiedBy(ownershipCtx);
        if (ownershipResult.IsFailure)
            return Fail<ProductVariantWriteAck>(ownershipResult.Error);

        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v =>
                v.VendorId == request.VendorId,
                cancellationToken: cancellationToken);
        if (vendorSnapshot == null)
            return Fail<ProductVariantWriteAck>(VendorSnapshotErrors.NotFound());

        var ctx = new ProductVariantRemoveImageContext(
            vendorSnapshot.IsActive,
            productSnapshot.CanEditContent);

        var result = productVariant.RemoveImage(
            ctx,
            request.ImageId,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return Fail<ProductVariantWriteAck>(result.Error);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(new ProductVariantWriteAck(productVariant.Id, productVariant.Version));
    }
}
