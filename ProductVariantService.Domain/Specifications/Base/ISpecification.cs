using migApp.Shared.Results;

namespace ProductVariantService.Domain.Specifications.Base;

public interface ISpecification<T>
{
    IResult IsSatisfiedBy(T candidate);
}