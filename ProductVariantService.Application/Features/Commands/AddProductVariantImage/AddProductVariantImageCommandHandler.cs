using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Interfaces.Data;
using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Specifications.Common;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.AddProductVariantImage;

public sealed class AddProductVariantImageCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<AddProductVariantImageCommand, IResult>
{
    public async Task<IResult> Handle(AddProductVariantImageCommand request, CancellationToken cancellationToken)
    {
        var productVariant = await context.ProductVariants
            .FirstOrDefaultAsync(p => p.Id == request.ProductVariantId, cancellationToken);
        if (productVariant == null)
            return Fail(ProductVariantErrors.NotFound());

        var productSnapshot = await context.ProductSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.ProductId == productVariant.ProductId,
                cancellationToken);
        if (productSnapshot is null)
            return Fail(ProductSnapshotErrors.NotFound());

        var ownershipCtx = new ProductVendorOwnershipContext(
            request.VendorId,
            productSnapshot.VendorId);

        var ownershipResult = ProductBelongsToVendorSpec.Instance.IsSatisfiedBy(ownershipCtx);
        if (ownershipResult.IsFailure)
            return ownershipResult;

        var vendorSnapshot = await context.VendorSnapshots
            .AsNoTracking()
            .FirstOrDefaultAsync(v =>
                v.VendorId == request.VendorId,
                cancellationToken: cancellationToken);
        if (vendorSnapshot == null)
            return Fail(VendorSnapshotErrors.NotFound());

        var ctx = new ProductVariantAddImageContext(
            vendorSnapshot.IsActive,
            productSnapshot.CanBeModified,
            productVariant.Images.Count);

        var buildResult = ProductVariantAddImageDataBuilder.Build(request);
        if (buildResult.IsFailure)
            return buildResult;

        var data = buildResult.Value;

        var result = productVariant.AddImage(
            ctx,
            data,
            timeProvider.GetUtcNow());
        if (result.IsFailure)
            return result;

        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
