using migApp.Shared.Results;
using ProductVariantService.Domain.Contexts;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Specifications.Base;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Domain.Specifications.Common;

public sealed class ProductBelongsToVendorSpec : Specification<ProductVendorOwnershipContext>
{
    public static readonly ProductBelongsToVendorSpec Instance = new();

    public override IResult IsSatisfiedBy(ProductVendorOwnershipContext ctx)
    {
        if (ctx.RequestVendorId != ctx.ProductVendorId)
            return Fail(ProductSnapshotErrors.DoesNotBelongToVendor());

        return Ok();
    }
}
