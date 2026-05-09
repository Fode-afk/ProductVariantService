using ProductVariantService.Domain.Abstractions;
using ProductVariantService.Domain.ValueObjects;

namespace ProductVariantService.Domain.Contexts;

public sealed record ProductVariantCreationContext(
    bool VendorIsActive,
    bool ProductCanBeModified,
    IReadOnlyCollection<VariantAttribute> Attributes) : IVendorContext, IProductContext, IAttributesContext;