using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Domain.Abstractions;

public interface IAttributesContext
{
    IReadOnlyCollection<VariantAttribute> Attributes { get; }
}
