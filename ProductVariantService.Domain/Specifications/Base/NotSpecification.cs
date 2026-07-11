using migApp.Shared.Results;

namespace ProductVariantService.Domain.Specifications.Base;

public sealed class NotSpecification<T>(
    ISpecification<T> inner) : Specification<T>
{
    public override IResult IsSatisfiedBy(T candidate)
    {
        var result = inner.IsSatisfiedBy(candidate);

        return result.IsSuccess
            ? ResultFactory.Fail(Error.InvalidArgument("NOT_SPEC_FAILED", "The specification was not satisfied."))
            : ResultFactory.Ok();
    }
}
