using MediatR;
using Microsoft.EntityFrameworkCore;
using migApp.Shared.Results;
using ProductVariantService.Application.Interfaces.Data;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Application.Features.Commands.UpdateProductInfo;

public sealed class UpdateProductInfoCommandHandler(
    IAppDbContext context,
    TimeProvider timeProvider) : IRequestHandler<UpdateProductInfoCommand, IResult>
{
    public async Task<IResult> Handle(UpdateProductInfoCommand request, CancellationToken cancellationToken)
    {
        //var product = await context.Products
        //    .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        //if (product == null)
        //    return Fail(ProductErrors.NotFound());
        //
        //var vendorSnapshot = await context.VendorSnapshots
        //    .AsNoTracking()
        //    .FirstOrDefaultAsync(v =>
        //        v.VendorId == request.VendorId,
        //        cancellationToken: cancellationToken);
        //if (vendorSnapshot == null)
        //    return Fail<VendorSnapshot>(VendorSnapshotErrors.NotFound());
        //
        //var productCardSnapshot = await context.ProductSnapshots
        //    .AsNoTracking()
        //    .FirstOrDefaultAsync(x =>
        //        x.ProductId == product.ProductCardId,
        //        cancellationToken);
        //if (productCardSnapshot is null)
        //    return Fail<ProductCardSnapshot>(ProductCardSnapshotErrors.NotFound());
        //
        //var buildResult = ProductUpdateInfoPolicy.Build(request);
        //if (buildResult.IsFailure)
        //    return buildResult;
        //
        //var data = buildResult.Value;
        //
        //var result = product.UpdateInfo(
        //    vendorSnapshot,
        //    productCardSnapshot,
        //    data.Name,
        //    data.Dimensions,
        //    data.Weight,
        //    timeProvider.GetUtcNow());
        //if (result.IsFailure)
        //    return result;
        //
        //await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
