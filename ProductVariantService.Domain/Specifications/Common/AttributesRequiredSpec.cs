using migApp.Shared.Results;
using ProductVariantService.Domain.Abstractions;
using ProductVariantService.Domain.Errors;
using ProductVariantService.Domain.Specifications.Base;
using static migApp.Shared.Results.ResultFactory;

namespace ProductVariantService.Domain.Specifications.Common;

public sealed class AttributesRequiredSpec<T> : Specification<T>
    where T : IAttributesContext
{
    public override IResult IsSatisfiedBy(T ctx)
    {
        if (ctx.Attributes.Count == 0)
            return Fail(ProductVariantErrors.AttributesRequired());

        return Ok();
    }
}
