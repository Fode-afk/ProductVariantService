using migApp.Shared.Results;
using ProductVariantService.Domain.Abstractions;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Specifications.Base;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Domain.Specifications.Common;

public sealed class VendorIsActiveSpec<T> : Specification<T>
    where T : IVendorContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (!ctx.VendorIsActive)
            return Fail(VendorSnapshotErrors.CannotModify());

        return Ok();
    }
}