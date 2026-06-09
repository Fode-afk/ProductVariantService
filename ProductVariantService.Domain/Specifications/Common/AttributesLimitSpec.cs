using migApp.Shared.Results;
using ProductVariantService.Domain.Abstractions;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Specifications.Base;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Domain.Specifications.Common;

public sealed class AttributesLimitSpec<T> : Specification<T>
    where T : IAttributesContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (ctx.Attributes.Count > Models.ProductVariant.MaxAttributes)
            return Fail(ProductVariantErrors.MaxAttributesReached(Models.ProductVariant.MaxAttributes));

        return Ok();
    }
}