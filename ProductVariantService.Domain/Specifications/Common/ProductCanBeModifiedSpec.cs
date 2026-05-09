using migApp.Shared.Results;
using ProductVariantService.Domain.Abstractions;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Specifications.Base;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Domain.Specifications.Common;

public sealed class ProductCanBeModifiedSpec<T> : Specification<T>
    where T : IProductContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (!ctx.ProductCanBeModified)
            return Fail(ProductSnapshotErrors.CannotModify());

        return Ok();
    }
}