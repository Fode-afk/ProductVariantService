using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Domain.Abstractions;

internal interface IAttributesContext
{
    IReadOnlyCollection<VariantAttribute> Attributes { get; }
}
