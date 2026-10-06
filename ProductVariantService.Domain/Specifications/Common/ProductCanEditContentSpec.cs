using migApp.Shared.Results;
using ProductVariantService.Domain.Abstractions;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Specifications.Base;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Domain.Specifications.Common;

public sealed class ProductCanEditContentSpec<T> : Specification<T>
    where T : IProductContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (!ctx.ProductCanEditContent)
            return Fail(ProductSnapshotErrors.CannotEditContent());

        return Ok();
    }
}